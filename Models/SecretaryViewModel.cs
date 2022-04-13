namespace WAP_PIS.Models;

public class SecretaryViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public ManagerViewModel Manager { get; set; }
}
