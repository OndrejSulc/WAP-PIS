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
public class UserManagementController : Controller
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

    private bool CheckIfUserIsCEO(Account user)
    {
        var manager = _db.Manager.Find(user);
        return manager != null && manager.IsCEO;
    }

    [HttpGet]
    public async Task<UserManagementViewModel> CreateNewUser(UserManagementViewModel umvm)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var searchUser = new Account(){Id=userId};
        var user = _db.Account.Find(searchUser);

        if( user == null || !CheckIfUserIsCEO(user))
        {
            umvm.Create_Status = false;
            umvm.Status_Message = "User is not CEO";
            return umvm;
        }

        if( umvm.IsSecretary ) //new acc is manager
        {
            if( umvm.ManagerForNewSecretary == null ) 
            {
                umvm.Create_Status = false;
                umvm.Status_Message = "Missing manager for new secretary";
                return umvm;
            }

            var newUser = new Account(){ UserName=umvm.Username};
            var result = await _um.CreateAsync(newUser);

            if(!result.Succeeded)
            {
                umvm.Create_Status = false;
                umvm.Status_Message = "Could not create new User: "+result.Errors.ToString();
                return umvm;
            }

            var newSecretary = new Secretary(){Account = newUser, Manager = umvm.ManagerForNewSecretary};
            _db.Secretary.Add(newSecretary);
        }

        else
        {
            var newUser = new Account(){ UserName=umvm.Username};
            var result = await _um.CreateAsync(newUser);

            if(!result.Succeeded)
            {
                umvm.Create_Status = false;
                umvm.Status_Message = "Could not create new User: "+result.Errors.ToString();
                return umvm;
            }

            var newManager = new Manager(){Account = newUser, IsCEO = false};
            _db.Manager.Add(newManager);
        }

        umvm.Create_Status = true;
        umvm.Status_Message = "New User added";
        return umvm;
    }

}
