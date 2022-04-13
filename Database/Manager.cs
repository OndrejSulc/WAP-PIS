using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;

[Table("Manager")]
public class Manager : Account
{
<<<<<<< Updated upstream
    public virtual Secretary? Secretary { get; set; }
=======
    [ForeignKey("Secretary.Id")]
    public Secretary? Secretary { get; set; }
>>>>>>> Stashed changes
    public bool IsCEO { get; set; }
    
    public virtual List<Meeting> Meetings { get; set; }
}
