using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WAP_PIS.Database;
public class Account : IdentityUser
{
    [Required]
    public DateTime Date_Of_Birth { get; set; }
}
