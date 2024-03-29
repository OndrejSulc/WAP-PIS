using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WAP_PIS.Models;

namespace WAP_PIS.Database;
public class Meeting
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public virtual Manager Owner { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime From { get; set; }
    public DateTime Until { get; set; }
    public virtual List<Manager> Attendees { get; set; }
    public virtual List<Notification> Notifications { get; set; }

}
