using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WAP_PIS.Models;
using System.Security.Claims;
using WAP_PIS.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace WAP_PIS.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class UserManagementController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<Account> _um;
    private IWebHostEnvironment _he;
    private SignInManager<Account> _sm;
    public UserManagementController(ApplicationDbContext applicationDbContext,
                         UserManager<Account> userManager,
                         IWebHostEnvironment webHostEnv,
                         SignInManager<Account> signInManager)
    {
        _db = applicationDbContext;
        _um = userManager;
        _he = webHostEnv;
        _sm = signInManager;
    }

    [HttpDelete]
    public async Task<UserManagementViewModel> DeleteUser([FromBody] UserManagementViewModel umvm)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _um.FindByIdAsync(userId);

        if(!(user is Manager manager) || !(manager.IsCEO) )
        {
            umvm.Status = false;
            umvm.Status_Message = "Only CEO can create new users";
            return umvm;
        }

        var userToDelete = await _um.FindByNameAsync(umvm.Username);
        var result = await _um.DeleteAsync(userToDelete);

        if(result.Succeeded)
        {
            umvm.Status = true;
            umvm.Status_Message = "User deleted.";
        }
        else
        {
            umvm.Status = false;
            umvm.Status_Message = "Error occured during delete";
        }
        return umvm;
    }

    [HttpPost]
    public async Task<UserManagementViewModelCreate> CreateNewUser([FromBody] UserManagementViewModelCreate umvm)
    {
        if(umvm == null )
        {
            umvm = new UserManagementViewModelCreate(){Status = false, Status_Message="null body of request"};
            return umvm;
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _um.FindByIdAsync(userId);

        if(!(user is Manager manager) || !(manager.IsCEO) )
        {
            umvm.Status = false;
            umvm.Status_Message = "Only CEO can create new users";
            return umvm;
        }

        if( umvm.IsSecretary )
        {
            return await CreateNewSecretaryAsync(umvm);
        }
        else
        {
            return await CreateNewManagerAsync(umvm);
        }

    }
    
    private async Task<UserManagementViewModelCreate> CreateNewSecretaryAsync(UserManagementViewModelCreate umvm)
    {
       
        if(umvm.ManagerIDForNewSecretary == null)
        {
            umvm.Status = false;
            umvm.Status_Message = "Missing manager";
            return umvm;
        }

        var new_Secretary_Manager_Account = await _um.FindByIdAsync(umvm.ManagerIDForNewSecretary);
        var new_Secretarys_Manager = new_Secretary_Manager_Account as Manager;
        if( (new_Secretary_Manager_Account) == null || (new_Secretarys_Manager) == null)
        {
            umvm.Status = false;
            umvm.Status_Message = "Wrong manager ID";
            return umvm;
        }

        var new_Secretary = new Secretary(){UserName = umvm.Username,
                                            Name = umvm.Name,
                                            Surname = umvm.Surname,
                                            Date_Of_Birth = umvm.Date_Of_Birth
        };

        new_Secretarys_Manager.Secretary = new_Secretary;
        new_Secretary.Manager = new_Secretarys_Manager;

        await _um.CreateAsync(new_Secretary,umvm.Password);
        await _um.UpdateAsync(new_Secretarys_Manager);

        umvm.Status = true;
        umvm.Status_Message = "New Secretary added";
        return umvm;
    }

    private async Task<UserManagementViewModelCreate> CreateNewManagerAsync(UserManagementViewModelCreate umvm)
    {
       
        var new_Manager = new Manager(){UserName = umvm.Username,
                                        Name = umvm.Name,
                                        Surname = umvm.Surname,
                                        Date_Of_Birth = umvm.Date_Of_Birth
        };

        await _um.CreateAsync(new_Manager,umvm.Password);

        umvm.Status = true;
        umvm.Status_Message = "New Secretary added";
        return umvm;
    }

}
