namespace WAP_PIS.Models;

public class UpdateMeetingViewModel
{
    public string? Title { get; set; } = null;
    public string? Description { get; set; } = null;
    public DateTime? From { get; set; } = null;
    public DateTime? Until { get; set; } = null;
}
