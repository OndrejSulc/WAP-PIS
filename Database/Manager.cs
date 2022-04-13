using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;

[Table("Manager")]
public class Manager : Account
{
    [ForeignKey("Secretary.Id")]
    public virtual Secretary Secretary { get; set; }
    public bool IsCEO { get; set; }
    
    public virtual List<Meeting> Meetings { get; set; }
}
