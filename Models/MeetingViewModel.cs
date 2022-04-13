namespace WAP_PIS.Models;

public class MeetingViewModel
{
    public int ID { get; set; }
    public ManagerViewModel Owner { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime From { get; set; }
    public DateTime Until { get; set; }
}
