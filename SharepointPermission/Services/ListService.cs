using Microsoft.SharePoint.Client;
using SharepointPermission.Interfaces;
using SharepointPermission.Models.List;

namespace SharepointPermission.Services;

public class ListService : IListService
{
    private readonly ClientContext _ctx;

    public ListService(ClientContext ctx)
    {
        _ctx = ctx;
    }

    public void StopInheritingPermissions(string listName)
    {
        var list = _ctx.Web.Lists.GetByTitle(listName);

        _ctx.Load(list);

        list.BreakRoleInheritance(true, true);

        _ctx.ExecuteQuery();
    }

    public void DeleteUniquePermission(string listName)
    {
        var list = _ctx.Web.Lists.GetByTitle(listName);

        list.ResetRoleInheritance();

        list.Update();
        _ctx.ExecuteQuery();
    }

    public void AddUserToPermissionList(AddUserToPermissionListInput addUserToPermissionListInput)
    {
        var list = _ctx.Web.Lists.GetByTitle(addUserToPermissionListInput.listName);

        var user = _ctx.Web.EnsureUser(addUserToPermissionListInput.username);

        var roleDefinition =
            _ctx.Site.RootWeb.RoleDefinitions.GetByName(addUserToPermissionListInput.permissionLevelName);

        var roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(_ctx) { roleDefinition };

        list.RoleAssignments.Add(user, roleDefinitionBindingCollection);

        list.Update();
        _ctx.ExecuteQuery();
    }
}