using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;
public class Secretary
{
    public Account? Account { get; set; }

    [ForeignKey("Manager")]
    public int Manager { get; set; }
    public bool IsCEO { get; set; }
}
