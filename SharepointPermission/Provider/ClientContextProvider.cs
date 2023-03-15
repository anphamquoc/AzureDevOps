using System.Security;

namespace SharepointPermission.Provider;

public class ClientContextProvider : IServiceProvider
{
    private string SiteUrl { get; }
    private string UserName { get; }
    private string Password { get; }

    public ClientContextProvider(string siteUrl, string userName, string password)
    {
        SiteUrl = siteUrl;
        UserName = userName;
        Password = password;
    }

    public object GetService(Type serviceType)
    {
        var authenticationManager = new AuthenticationManager();
        var secureString = new SecureString();
        foreach (var c in Password) secureString.AppendChar(c);
        return authenticationManager.GetContext(new Uri(SiteUrl), UserName, secureString);
    }
}