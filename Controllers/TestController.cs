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
    private readonly UserManager<IdentityUser> _um;
    private IWebHostEnvironment _he;

    public TestController(ApplicationDbContext applicationDbContext,
                         UserManager<IdentityUser> userManager,
                         IWebHostEnvironment webHostEnv)
    {
        _db = applicationDbContext;
        _um = userManager;
        _he = webHostEnv;
    }

    public string Index()
    {
        Console.WriteLine("*--------");
        Console.WriteLine(User);
        Console.WriteLine("--------*");

        if (!User.Identity.IsAuthenticated)
        {
            return "user not authetnticated";
        }
       
        var date = new DateTime(1999,1,5);
        var newAcc = new Account(){
            Login = "user",
            Password = "user",
            Name = "UserName",
            Surname = "UserSur",
            Date_Of_Birth = date};

        _db.Account.Add(newAcc);
        _db.SaveChanges();

        return "User Autheticated API Test Index call finished";
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
