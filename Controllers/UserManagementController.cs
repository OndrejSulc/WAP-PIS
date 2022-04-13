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
using WAP_PIS.Extensions;

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

    [HttpGet]
    [Authorize("CEO")]
    public UserManagementGetAllUsersViewModel GetAllUsers()
    {
        var managers = _db.Manager.ToList().Select( m => m.ToViewModel() ).ToList();
        var secretaries = _db.Secretary.ToList().Select( s => s.ToViewModel() ).ToList();

        var vm = new UserManagementGetAllUsersViewModel(){Managers = managers, Secretaries = secretaries};

        return vm;
    }


    [HttpDelete]
    public async Task<UserManagementDeleteViewModel> DeleteUser([FromBody] UserManagementDeleteViewModel umvm)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _um.FindByIdAsync(userId);

        if(!(user is Manager manager) || !(manager.IsCEO) )
        {
            umvm.Status = false;
            umvm.Status_Message = "Only CEO can delete new users";
            return umvm;
        }

        var userToDelete = await _um.FindByIdAsync(umvm.UserID);

        if(userToDelete == null)
        {
            umvm.Status = false;
            umvm.Status_Message = "User with this ID was not found";
            return umvm;
        }

       
        if(userToDelete is Manager managerToDelete)
        {
            if(managerToDelete.Secretary != null)
            {
                var his_secretaries = _db.Secretary.Where(s => s.Manager == managerToDelete);
                _db.Secretary.RemoveRange(his_secretaries);
            }
        }

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
    public async Task<UserManagementCreateViewModel> CreateNewUser([FromBody] UserManagementCreateViewModel umvm)
    {
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
    
    private async Task<UserManagementCreateViewModel> CreateNewSecretaryAsync(UserManagementCreateViewModel umvm)
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
            
            var result = await _um.CreateAsync(new_Secretary,umvm.Password);
            if(result.Succeeded)
            {
                umvm.Status = true;
                umvm.Status_Message = "New Secretary added";
            }
            else
            {
                umvm.Status = false;
                umvm.Status_Message = "Provided Username already in use.";
            }
        }
        else
        {
            umvm.Status = false;
            umvm.Status_Message = "Failed to bind manager of new secretary";
        }
        
        return umvm;
    }

    private async Task<UserManagementCreateViewModel> CreateNewManagerAsync(UserManagementCreateViewModel umvm)
    {
       
        var new_Manager = new Manager(){UserName = umvm.Username,
                                        Name = umvm.Name,
                                        Surname = umvm.Surname,
                                        Date_Of_Birth = umvm.Date_Of_Birth
        };

        
        var result = await _um.CreateAsync(new_Manager,umvm.Password);

        if(result.Succeeded)
        {
            umvm.Status = true;
            umvm.Status_Message = "New Manager added";
        }
        else
        {
            umvm.Status = false;
            umvm.Status_Message = "Provided Username already in use.";
        }
        
        return umvm;
    }

}
