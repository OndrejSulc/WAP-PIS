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
    public async Task<string> CreateTestingUsers()
    {
        var ceo = new Manager()
        {
            UserName = "ceo",
            Email = "",
            Name = "I.M.",
            Surname = "Tired",
            IsCEO = true
        };

        var man1 = new Manager()
        {
            UserName = "man1",
            Email = "",
            Name = "Man1 Anita",
            Surname = "Hugh"
        };

        var man2 = new Manager()
        {
            UserName = "man2",
            Email = "",
            Name = "Man2 Mr.",
            Surname = "Blue Sky"
        };

        var sec1 = new Secretary()
        {
            UserName = "sec1",
            Email = "",
            Name = "Sec1 Hugh",
            Surname = "N. Cry",
            Manager = man1
        };

        var sec2 = new Secretary()
        {
            UserName = "sec2",
            Email = "",
            Name = "Sec2 Electra",
            Surname = "Orchestra",
            Manager = man2
        };

         var secceo = new Secretary()
        {
            UserName = "secceo",
            Email = "",
            Name = "SecCeo Ivana",
            Surname = "B. Withew",
            Manager = ceo
        };

        var password = "password";

        var result0 = await _um.CreateAsync(ceo, password);
        var result1 = await _um.CreateAsync(man1, password);
        var result2 = await _um.CreateAsync(man2, password);
        var result3 = await _um.CreateAsync(sec1, password);
        var result4 = await _um.CreateAsync(sec2, password);
        var result5 = await _um.CreateAsync(secceo, password);

        if (result0.Succeeded &&
            result1.Succeeded &&
            result2.Succeeded &&
            result3.Succeeded &&
            result4.Succeeded &&
            result5.Succeeded)
        {
            return "Test users created.";

        }
        return "FAILED to create test users";
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
