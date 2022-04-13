using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WAP_PIS.Database;
using WAP_PIS.Extensions;
using WAP_PIS.Models;
using WAP_PIS.SignalRHubs;

namespace WAP_PIS.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> _logger;
    private readonly ApplicationDbContext appDbContext;
    private readonly IHubContext<NotificationHub> notificationHub;

    public NotificationController(ILogger<NotificationController> logger, ApplicationDbContext applicationDbContext, IHubContext<NotificationHub> notificationHub)
    {
        _logger = logger;
        appDbContext = applicationDbContext;
        this.notificationHub = notificationHub;
    }

    [HttpGet]
    public GetNotificationsViewModel GetNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var relevantNotifications = appDbContext.Notification
            .Where(notification => notification.Recipient.Id == userId)
            .ToList()
            .Select(notification => notification.ToViewModel())
            .ToList();

        
        
        return new GetNotificationsViewModel
        {
            Notifications = relevantNotifications
        };
    }

    [HttpPost]
    public IActionResult DismissNotification(int notificationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notification = appDbContext.Notification
            .Include(notification => notification.Recipient)
            .SingleOrDefault(notification => notification.ID == notificationId);

        if (notification == null)
        {
            return NotFound($"Notification with id: {notificationId} not found");
        }
        
        if (notification.Recipient.Id != userId)
        {
            return Unauthorized("You cannot dismiss notifications, which are not yours");
        }

        appDbContext.Notification.Remove(notification);
        appDbContext.SaveChanges();

        return Ok();
    }
}
