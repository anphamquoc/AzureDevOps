using Microsoft.SharePoint.Client;
using SharepointPermission.Interfaces;

namespace SharepointPermission.Services;

public class SiteService : ISiteService
{
    private readonly ClientContext _ctx;

    public SiteService(ClientContext ctx)
    {
        _ctx = ctx;
    }

    public Dictionary<int, string> GetAllPermissionKind()
    {
        var permissionKind = new Dictionary<int, string>();

        foreach (var permission in Enum.GetValues(typeof(PermissionKind)))
            permissionKind.Add((int)permission, permission.ToString());

        return permissionKind;
    }

    public void CreateNewPermissionLevel(string permissionName, IEnumerable<PermissionKind> roles)
    {
        var basePermissions = new BasePermissions();

        foreach (var role in roles) basePermissions.Set(role);

        var roleDefCreationInfo = new RoleDefinitionCreationInformation
        {
            Name = permissionName,
            BasePermissions = basePermissions
        };

        var roleDefinition = _ctx.Site.RootWeb.RoleDefinitions.Add(roleDefCreationInfo);

        _ctx.Load(roleDefinition);
        _ctx.ExecuteQuery();
    }

    public void CreateNewGroupPermission(string groupName)
    {
        var groupCreationInfo = new GroupCreationInformation
        {
            Title = groupName,
            Description = "Group for " + groupName
        };

        var group = _ctx.Site.RootWeb.SiteGroups.Add(groupCreationInfo);
        _ctx.Load(group);
        _ctx.ExecuteQuery();
    }

    public void AddPermissionLevelToGroup(string groupName, string permissionLevelName)
    {
        var permissionLevel = _ctx.Site.RootWeb.RoleDefinitions.GetByName(permissionLevelName);
        var group = _ctx.Site.RootWeb.SiteGroups.GetByName(groupName);

        var rolDefCollection = new RoleDefinitionBindingCollection(_ctx) { permissionLevel };

        _ctx.Site.RootWeb.RoleAssignments.Add(group, rolDefCollection);

        _ctx.ExecuteQuery();
    }

    public void AddUserToGroupPermission(string username, string groupName)
    {
        var group = _ctx.Web.SiteGroups.GetByName(groupName);

        var user = _ctx.Web.EnsureUser(username);

        _ctx.Load(group);
        _ctx.Load(user);
        _ctx.ExecuteQuery();

        group.Users.AddUser(user);

        group.Update();
        _ctx.ExecuteQuery();
    }
}