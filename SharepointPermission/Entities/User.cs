using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharepointPermission.Entities;

[Table("User")]
public class User
{
    [Key] public int Id { get; set; }

    [Required] public string Username { get; set; }

    [Required] public string Password { get; set; }
}