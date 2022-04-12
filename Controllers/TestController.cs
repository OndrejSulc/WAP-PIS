using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WAP_PIS.Models;
using WAP_PIS.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;

namespace WAP_PIS.Controllers;

public class TestController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<Account> _um;
    private IWebHostEnvironment _he;
    private SignInManager<Account> _sm;
    public TestController(ApplicationDbContext applicationDbContext,
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
    public async Task<string> CreateNewUser()
    {
        var user = new Account(){UserName = "user", Email = "user@email.cz"};
        var result = await _um.CreateAsync(user, "password");

        if(result.Succeeded)
        {
            await _sm.SignInAsync(user,false);
            return "New USER CREATED";

        }  
        return "FAILED to create new user";

    }

}
