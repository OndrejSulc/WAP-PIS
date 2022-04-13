using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;

[Table("Secretary")]
public class Secretary : Account
{
    public Manager Manager { get; set; }
}
