using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WAP_PIS.Database;
using Microsoft.AspNetCore.Identity;

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
    public async Task<string> CreateNewUser(
        string type = "CEO",
        string username = "user",
        string name = "CEO jméno",
        string surname = "Testovič",
        string email = "user@email.cz",
        string password = "password")
    {
        Account user = type switch
        {
            "Manager" => new Manager()
            {
                UserName = username,
                Email = email,
                Name = name,
                Surname = surname
            },
            "CEO" => new Manager()
            {
                UserName = username,
                Email = email,
                Name = name,
                Surname = surname,
                IsCEO = true
            },
            "Secretary" => new Secretary()
            {
                UserName = username,
                Email = email,
                Name = name,
                Surname = surname,
            },
            _ => new Manager()
            {
                UserName = username,
                Email = email,
                Name = name,
                Surname = surname,
                IsCEO = true
            }
        };
        var result = await _um.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _sm.SignInAsync(user, false);
            return "New USER CREATED";

        }
        return "FAILED to create new user";

    }

    [HttpGet]
    public async Task<string> GetClaims()
    {
        return User.Claims.ToString();
    }

    [Authorize(Policy = "CEO")]
    [HttpGet]
    public string TestCEO()
    {
        return "You are CEO";
    }


}
