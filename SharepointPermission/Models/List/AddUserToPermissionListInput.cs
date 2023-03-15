using System.ComponentModel.DataAnnotations;

namespace SharepointPermission.Models.List;

public class AddUserToPermissionListInput
{
    [Required]
    public string listName { get; set; }

    [Required]
    public string username { get; set; }

    [Required]
    public string permissionLevelName { get; set; }
}