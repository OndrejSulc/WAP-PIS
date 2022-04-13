using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WAP_PIS.Models;
using WAP_PIS.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace WAP_PIS.Controllers;

public class AuthenticationController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<Account> _um;
    private IWebHostEnvironment _he;
    private SignInManager<Account> _sm;
    public AuthenticationController(ApplicationDbContext applicationDbContext,
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
    public bool CheckLogin()
    {
        if( _sm.IsSignedIn(User))
        {
            return true;
        }
        return false;
    }

    [HttpPost]
    public async Task<LoginViewModel> Login([FromBody] LoginViewModel lwm)
    {

        var user = await _um.FindByNameAsync(lwm.Login);

        if(user == null)
        {
            lwm.Successful_Authentication = false;
            return lwm;
        }

        var signInResult = await _sm.PasswordSignInAsync(user, lwm.Password, false, false);

        if( signInResult.Succeeded)
        {
            lwm.Successful_Authentication = true;
        }
        else
        {
            lwm.Successful_Authentication = false;
        }

        
        var managers = _db.Manager.Where( m => m == user);
        if(managers.Count() != 0)
        {
            var manager = managers.First();
            lwm.IsCEO = manager.IsCEO;
        }
        else
        {
            lwm.IsCEO = false;
        }
        return lwm;
    }

    [HttpPost]
    public async Task<bool> Logout()
    {
        Console.WriteLine(_sm.IsSignedIn(User));
        if( _sm.IsSignedIn(User))
        {
            await _sm.SignOutAsync();
            return true;
        }
        return false;
    }

}
