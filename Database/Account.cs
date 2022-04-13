using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WAP_PIS.Database;

[Table("Account")]
public class Account : IdentityUser
{
    public string Name { get; set; }
    public string Surname { get; set; }
    [Required]
    public DateTime Date_Of_Birth { get; set; }
}
