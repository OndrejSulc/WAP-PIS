using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;

[Table("Secretary")]
public class Secretary : Account
{
    [ForeignKey("Manager.Id")]
    public virtual Manager Manager { get; set; }
}
