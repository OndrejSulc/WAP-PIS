using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WAP_PIS.Models;
using WAP_PIS.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Text.Json;
using WAP_PIS.Authorization;
using WAP_PIS.Extensions;

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
    public async Task<CheckLoginResultViewModel> CheckLogin()
    {
        if (!_sm.IsSignedIn(User))
            return new CheckLoginResultViewModel
            {
                LoggedIn = false,
                User = null
            };

        var account = await _um.GetUserAsync(User);
        var accountViewModel = account.ToAccountViewModel();

        return new CheckLoginResultViewModel
        {
            LoggedIn = true,
            User = accountViewModel
        };
    }

    [HttpPost]
    public async Task<LoginViewModel> Login([FromBody] LoginViewModel lwm)
    {

        var user = await _um.FindByNameAsync(lwm.Login);

        if (user == null)
        {
            lwm.Successful_Authentication = false;
            return lwm;
        }

        var signInResult = await _sm.PasswordSignInAsync(user, lwm.Password, false, false);

        if (signInResult.Succeeded)
        {
            lwm.Successful_Authentication = true;
        }
        else
        {
            lwm.Successful_Authentication = false;
        }

        lwm.Role = user.GetRole();
        return lwm;
    }

    [HttpPost]
    public async Task<bool> Logout()
    {
        if (_sm.IsSignedIn(User))
        {
            await _sm.SignOutAsync();
            return true;
        }
        return false;
    }

}
