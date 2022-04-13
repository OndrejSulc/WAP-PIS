using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;
public class Notification
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime Date { get; set; }
    public virtual Manager Recipient { get; set; }
    public virtual Meeting Meeting { get; set; }
}
