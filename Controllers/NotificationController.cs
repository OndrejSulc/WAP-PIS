using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAP_PIS.Database;
using WAP_PIS.Extensions;
using WAP_PIS.Models;

namespace WAP_PIS.Controllers;

public class NotificationController : Controller
{
    private readonly ILogger<NotificationController> _logger;
    private readonly ApplicationDbContext appDbContext;

    public NotificationController(ILogger<NotificationController> logger, ApplicationDbContext applicationDbContext)
    {
        _logger = logger;
        appDbContext = applicationDbContext;
    }

    [HttpGet]
    public GetNotificationsViewModel GetNotifications()
    {
        var userId = 0; //Todo: Get user id from identity framework
        var relevantNotifications = appDbContext.Notification
            .Where(notification => notification.Recipient.ID != userId)
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
        var userId = 0; //Todo: Get user id from identity framework
        var notification = appDbContext.Notification
            .Include(notification => notification.Recipient)
            .SingleOrDefault(notification => notification.ID == notificationId);

        if (notification == null)
        {
            return NotFound($"Notification with id: {notificationId} not found");
        }
        
        if (notification.Recipient.ID != userId)
        {
            return Unauthorized("You cannot dismiss notifications, which are not yours");
        }

        appDbContext.Notification.Remove(notification);

        return Ok();
    }
}
