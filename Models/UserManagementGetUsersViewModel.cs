using WAP_PIS.Database;
namespace WAP_PIS.Models;


public class UserManagementGetAllUsersViewModel
{
    public List<ManagerViewModel> Managers { get; set; }
    public List<SecretaryViewModel> Secretaries { get; set; }
}
