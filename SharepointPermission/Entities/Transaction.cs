using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharepointPermission.Entities;

public class Transaction
{
    [Key] public int Id { get; set; }

    [Required] public string CreditCardNumber { get; set; }

    [Required] public string CreditCardType { get; set; }

    public int Total { get; set; }


    [ForeignKey("UserId")] [Required] public int UserId { get; set; }

    public User User { get; set; }
}