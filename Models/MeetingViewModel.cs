using WAP_PIS.Database;

namespace WAP_PIS.Models;

public class MeetingViewModel
{
    
    // {
    //     ID: "2",
    //     From: "20220508T1000",
    //     Until: "20220508T1100",
    //     Title: "Test meeting OZNUK session num 2",
    //     Description: "Atos Portos Divočákos",
    //     Owner: "1"
    // },
    
    public int ID { get; set; }
    public Manager Owner { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime From { get; set; }
    public DateTime Until { get; set; }
}
