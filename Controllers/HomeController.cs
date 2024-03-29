﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WAP_PIS.Models;
using WAP_PIS.Database;

namespace WAP_PIS.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext appDbContext;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public void Index()
    {
        Response.Redirect("webclient/html/index.html");
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
