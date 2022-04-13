using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;

[Table("Manager")]
public class Manager : Account
{
    [ForeignKey("Secretary.Id")]
    public Secretary Secretary { get; set; }
    public bool IsCEO { get; set; }
    
    public List<Meeting> Meetings { get; set; }
}
