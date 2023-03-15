using Microsoft.SharePoint.Client;

namespace SharepointPermission.Interfaces;

public interface ISiteService
{
    public Dictionary<int, string> GetAllPermissionKind();
    public void AddUserToGroupPermission(string username, string groupName);
    public void CreateNewPermissionLevel(string permissionLevelName, IEnumerable<PermissionKind> roles);
    public void CreateNewGroupPermission(string groupName);
    public void AddPermissionLevelToGroup(string groupName, string permissionLevelName);
}