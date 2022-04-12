using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;
public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    
    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Surname { get; set; }

    public DateTime? Date_Of_Birth { get; set; }
    
    public ICollection<Meeting> Meetings { get; set; }
}
