using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;

[Table("Secretary")]
public class Secretary : Account
{
    public virtual Manager Manager { get; set; }
}
