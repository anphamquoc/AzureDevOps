using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.SharePoint.Client;

namespace SharepointPermission;

public class AuthenticationManager : IDisposable
{
    private const string tokenEndpoint =
        "https://login.microsoftonline.com/common/oauth2/token";

    private const string defaultAADAppId = "d599cdf5-8e12-4e82-8e43-3333ae533efe";
    private static readonly HttpClient httpClient = new();

    // Token cache handling
    private static readonly SemaphoreSlim semaphoreSlimTokens = new(1);
    private readonly ConcurrentDictionary<string, string> tokenCache = new();
    private bool disposedValue;
    private AutoResetEvent tokenResetEvent;

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ClientContext GetContext(Uri web, string userPrincipalName, SecureString userPassword)
    {
        var context = new ClientContext(web);

        context.ExecutingWebRequest += (sender, e) =>
        {
            var accessToken = EnsureAccessTokenAsync(new Uri($"{web.Scheme}://{web.DnsSafeHost}"), userPrincipalName,
                new NetworkCredential(string.Empty, userPassword).Password).GetAwaiter().GetResult();
            e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + accessToken;
        };

        return context;
    }


    public async Task<string> EnsureAccessTokenAsync(Uri resourceUri, string userPrincipalName, string userPassword)
    {
        var accessTokenFromCache = TokenFromCache(resourceUri, tokenCache);
        if (accessTokenFromCache == null)
        {
            await semaphoreSlimTokens.WaitAsync().ConfigureAwait(false);
            try
            {
                // No async methods are allowed in a lock section
                var accessToken = await AcquireTokenAsync(resourceUri, userPrincipalName, userPassword)
                    .ConfigureAwait(false);
                Console.WriteLine(
                    $"Successfully requested new access token resource {resourceUri.DnsSafeHost} for user {userPrincipalName}");
                AddTokenToCache(resourceUri, tokenCache, accessToken);

                // Register a thread to invalidate the access token once's it's expired
                tokenResetEvent = new AutoResetEvent(false);
                var wi = new TokenWaitInfo();
                wi.Handle = ThreadPool.RegisterWaitForSingleObject(
                    tokenResetEvent,
                    async (state, timedOut) =>
                    {
                        if (!timedOut)
                        {
                            var internalWaitToken = (TokenWaitInfo)state;
                            if (internalWaitToken.Handle != null) internalWaitToken.Handle.Unregister(null);
                        }
                        else
                        {
                            try
                            {
                                // Take a lock to ensure no other threads are updating the SharePoint Access token at this time
                                await semaphoreSlimTokens.WaitAsync().ConfigureAwait(false);
                                RemoveTokenFromCache(resourceUri, tokenCache);
                                Console.WriteLine(
                                    $"Cached token for resource {resourceUri.DnsSafeHost} and user {userPrincipalName} expired");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(
                                    $"Something went wrong during cache token invalidation: {ex.Message}");
                                RemoveTokenFromCache(resourceUri, tokenCache);
                            }
                            finally
                            {
                                semaphoreSlimTokens.Release();
                            }
                        }
                    },
                    wi,
                    (uint)CalculateThreadSleep(accessToken).TotalMilliseconds,
                    true
                );

                return accessToken;
            }
            finally
            {
                semaphoreSlimTokens.Release();
            }
        }

        Console.WriteLine(
            $"Returning token from cache for resource {resourceUri.DnsSafeHost} and user {userPrincipalName}");
        return accessTokenFromCache;
    }

    private async Task<string> AcquireTokenAsync(Uri resourceUri, string username, string password)
    {
        var resource = $"{resourceUri.Scheme}://{resourceUri.DnsSafeHost}";

        var clientId = defaultAADAppId;
        var body =
            $"resource={resource}&client_id={clientId}&grant_type=password&username={HttpUtility.UrlEncode(username)}&password={HttpUtility.UrlEncode(password)}";
        using (var stringContent = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"))
        {
            var result = await httpClient.PostAsync(tokenEndpoint, stringContent).ContinueWith(response =>
            {
                return response.Result.Content.ReadAsStringAsync().Result;
            }).ConfigureAwait(false);

            var tokenResult = JsonSerializer.Deserialize<JsonElement>(result);
            Console.WriteLine(tokenResult);
            var token = tokenResult.GetProperty("access_token").GetString();
            return token;
        }
    }

    private static string TokenFromCache(Uri web, ConcurrentDictionary<string, string> tokenCache)
    {
        if (tokenCache.TryGetValue(web.DnsSafeHost, out var accessToken)) return accessToken;

        return null;
    }

    private static void AddTokenToCache(Uri web, ConcurrentDictionary<string, string> tokenCache, string newAccessToken)
    {
        if (tokenCache.TryGetValue(web.DnsSafeHost, out var currentAccessToken))
            tokenCache.TryUpdate(web.DnsSafeHost, newAccessToken, currentAccessToken);
        else
            tokenCache.TryAdd(web.DnsSafeHost, newAccessToken);
    }

    private static void RemoveTokenFromCache(Uri web, ConcurrentDictionary<string, string> tokenCache)
    {
        tokenCache.TryRemove(web.DnsSafeHost, out var currentAccessToken);
    }

    private static TimeSpan CalculateThreadSleep(string accessToken)
    {
        var token = new JwtSecurityToken(accessToken);
        var lease = GetAccessTokenLease(token.ValidTo);
        lease = TimeSpan.FromSeconds(lease.TotalSeconds - TimeSpan.FromMinutes(5).TotalSeconds > 0
            ? lease.TotalSeconds - TimeSpan.FromMinutes(5).TotalSeconds
            : lease.TotalSeconds);
        return lease;
    }

    private static TimeSpan GetAccessTokenLease(DateTime expiresOn)
    {
        var now = DateTime.UtcNow;
        var expires = expiresOn.Kind == DateTimeKind.Utc ? expiresOn : TimeZoneInfo.ConvertTimeToUtc(expiresOn);
        var lease = expires - now;
        return lease;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                if (tokenResetEvent != null)
                {
                    tokenResetEvent.Set();
                    tokenResetEvent.Dispose();
                }

            disposedValue = true;
        }
    }

    internal class TokenWaitInfo
    {
        public RegisteredWaitHandle Handle;
    }
}