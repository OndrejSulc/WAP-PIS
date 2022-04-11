using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WAP_PIS.Models;
using WAP_PIS.Database;

namespace WAP_PIS.Controllers;

public class TestController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext appDbContext;

    public TestController(ILogger<HomeController> logger,ApplicationDbContext applicationDbContext)
    {
        _logger = logger;
        appDbContext = applicationDbContext;
    }

    public string Index()
    {
        var date = new DateTime(1999,1,5);
        var newAcc = new Account(){
            Login = "user",
            Password = "user",
            Name = "UserName",
            Surname = "UserSur",
            Date_Of_Birth = date};

        appDbContext.Account.Add(newAcc);
        appDbContext.SaveChanges();

        return "API Test Index call finished";
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
