namespace WAP_PIS.Database;

public class Manager : Account
{
    public Secretary? Secretary { get; set; }
    public bool IsCEO { get; set; }
    public ICollection<Meeting> Meetings { get; set; }
}
