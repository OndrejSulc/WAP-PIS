using WAP_PIS.Database;
namespace WAP_PIS.Models;


public class UserManagementCreateViewModel
{
     public string Username { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime Date_Of_Birth { get; set; }
    public bool IsSecretary { get; set; }
    public string? ManagerIDForNewSecretary { get; set; }
    public string? Status_Message { get; set; }
    public bool? Status { get; set; }
}
