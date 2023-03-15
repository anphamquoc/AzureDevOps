using SharepointPermission.Models.List;

namespace SharepointPermission.Interfaces;

public interface IListService
{
    public void StopInheritingPermissions(string listName);
    public void AddUserToPermissionList(AddUserToPermissionListInput addUserToPermissionListInput);
    public void DeleteUniquePermission(string listName);
}