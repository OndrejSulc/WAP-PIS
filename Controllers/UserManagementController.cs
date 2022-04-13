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

        var managerAcc = await _um.FindByIdAsync(umvm.ManagerIDForNewSecretary);
        if(managerAcc is Manager managerOfNewSecretary)
        {
            var new_Secretary = new Secretary(){UserName = umvm.Username,
                                            Name = umvm.Name,
                                            Surname = umvm.Surname,
                                            Date_Of_Birth = umvm.Date_Of_Birth,
                                            Manager = managerOfNewSecretary
            };
            await _um.CreateAsync(new_Secretary,umvm.Password);
            umvm.Status = true;
            umvm.Status_Message = "New Secretary added";
        }
        else
        {
            umvm.Status = false;
            umvm.Status_Message = "Failed to bind manager of new secretary";
        }
        
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
