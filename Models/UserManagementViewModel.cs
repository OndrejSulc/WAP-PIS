using WAP_PIS.Database;
namespace WAP_PIS.Models;


public class UserManagementViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }

    public bool IsSecretary { get; set; }
    public Manager? ManagerForNewSecretary { get; set; }

    public string? Status_Message { get; set; }
    public bool? Create_Status { get; set; }
}