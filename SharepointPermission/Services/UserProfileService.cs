using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using SharepointPermission.Entities;
using SharepointPermission.Interfaces;
using User = SharepointPermission.Entities.User;

namespace SharepointPermission.Services;

public class UserProfileService : IUserProfileService
{
    private readonly ClientContext _ctx;
    private readonly MyDbContext _dbContext;

    public UserProfileService(ClientContext ctx, MyDbContext dbContext)
    {
        _ctx = ctx;
        _dbContext = dbContext;
    }

    public List<User> GetAllUsers()
    {
        return _dbContext.Users.ToList();
    }

    public void UpdateUserPropertySingleValue(string accountName, Dictionary<string, string> valuesUpdated)
    {
        var peopleManager = new PeopleManager(_ctx);
        var personProperties = peopleManager.GetMyProperties();
        _ctx.Load(personProperties);
        _ctx.ExecuteQuery();

        foreach (var valueUpdated in valuesUpdated)
        {
            if (!personProperties.UserProfileProperties.ContainsKey(valueUpdated.Key))
                throw new Exception($"Property {valueUpdated.Key} not found");

            peopleManager.SetSingleValueProfileProperty(accountName, valueUpdated.Key, valueUpdated.Value);
        }

        _ctx.ExecuteQuery();
    }

    public void UpdateUserPropertyMultiValue(string accountName, Dictionary<string, List<string>> valuesUpdated)
    {
        var peopleManager = new PeopleManager(_ctx);
        var personProperties = peopleManager.GetMyProperties();
        _ctx.Load(personProperties);
        _ctx.ExecuteQuery();

        foreach (var key in personProperties.UserProfileProperties.Keys)
            Console.WriteLine(key + " " + personProperties.UserProfileProperties[key]);

        foreach (var valueUpdated in valuesUpdated)
        {
            if (!personProperties.UserProfileProperties.ContainsKey(valueUpdated.Key))
                throw new Exception($"Property {valueUpdated.Key} not found");

            peopleManager.SetMultiValuedProfileProperty(accountName, valueUpdated.Key, valueUpdated.Value);
        }

        _ctx.ExecuteQuery();
    }

    public List<Dictionary<string, string>> ListSomePropertiesOfAllUsers()
    {
        var result = new List<Dictionary<string, string>>();
        var users = _ctx.Web.SiteUsers;

        _ctx.Load(users);
        _ctx.ExecuteQuery();

        // list all users

        foreach (var user in users)
        {
            var item = new Dictionary<string, string>();
            item.Add("Id", user.Id.ToString());
            item.Add("LoginName", user.LoginName);
            item.Add("Title", user.Title);
            item.Add("Email", user.Email);
            result.Add(item);
        }

        return result;
    }
}