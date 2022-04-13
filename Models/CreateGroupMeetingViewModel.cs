namespace WAP_PIS.Models;

public class CreateGroupMeetingViewModel: CreateMeetingViewModel
{
    public string[] Attendees { get; set; } = Array.Empty<string>();
}
