
namespace WAP_PIS.Models;

public class NotificationViewModel
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime Date { get; set; }
    public MeetingViewModel Meeting { get; set; }
}
