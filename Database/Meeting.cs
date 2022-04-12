using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WAP_PIS.Database;
public class Meeting
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [ForeignKey("Manager")]
    public int Manager { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime From { get; set; }
    public DateTime Until { get; set; }
}
