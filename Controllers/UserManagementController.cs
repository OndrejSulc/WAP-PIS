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

    /*[HttpPost]
    public async Task<UserManagementViewModel> CreateNewUser(UserManagementViewModel umvm)
    {
        var user = await _um.FindByNameAsync(umvm.Username);

        if(!(user is Manager manager) || !(manager.IsCEO) )
        {
            umvm.Create_Status = false;
            umvm.Status_Message = "User is not CEO";
            return umvm;
        }

        if(umvm.IsSecretary)
        {
            if(umvm.ManagerForNewSecretary == null)
            {
                
            }

            var newUser = new Secretary(){ UserName = umvm.Username,
                                           Name = umvm.Name,
                                           Surname = umvm.Surname,
                                           Date_Of_Birth = umvm.Date_Of_Birth
            };
        }

        umvm.Create_Status = true;
        umvm.Status_Message = "New User added";
        return umvm;
    }*/

}
