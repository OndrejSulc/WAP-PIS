using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;
public class Manager
{
    [Required]
    public Account Account { get; set; }

    [ForeignKey("Secretary")]
    public int Secretary { get; set; }
    
    public bool IsCEO { get; set; }
}
