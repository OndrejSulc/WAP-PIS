using System.ComponentModel.DataAnnotations;

namespace WAP_PIS.Models;

public class CreateMeetingViewModel
{
    [Required]
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    [Required]
    public DateTime From { get; set; }
    [Required]
    public DateTime Until { get; set; }
    public int[] Attendees { get; set; } = Array.Empty<int>();
}
