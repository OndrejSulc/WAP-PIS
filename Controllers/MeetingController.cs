using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WAP_PIS.Models;
using WAP_PIS.Database;
using WAP_PIS.Extensions;
using WAP_PIS.SignalRHubs;

namespace WAP_PIS.Controllers;

/// <summary>
/// Controller for managing meetings
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class MeetingController : ControllerBase
{
    private readonly ILogger<MeetingController> _logger;
    private readonly ApplicationDbContext appDbContext;
    private readonly IHubContext<NotificationHub> notificationHub;
    private readonly UserManager<Account> userManager;

    public MeetingController(ILogger<MeetingController> logger, ApplicationDbContext applicationDbContext, IHubContext<NotificationHub> notificationHub, UserManager<Account> userManager)
    {
        _logger = logger;
        appDbContext = applicationDbContext;
        this.notificationHub = notificationHub;
        this.userManager = userManager;
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMeeting(int meetingId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var meeting = appDbContext.Meeting
            .Include(meeting => meeting.Attendees)
            .Include(meeting => meeting.Owner)
            .SingleOrDefault(meeting => meeting.ID == meetingId);
        if (meeting == null)
        {
            return NotFound("Meeting with id: " + meetingId + " was not found.");
        }
        //Todo: Check for update permissions. Secretary can remove managers meetings. CEO can edit everything
        if (meeting.Owner.Id != userId) //Todo: More sophisticated permission checking needed
        {
            return Unauthorized("You do not have permissions for this action.");
        }
    
        foreach (var manager in meeting.Attendees)
        {
            await NotifyUser(manager, "Meeting cancelled", "Meeting, which you attend was cancelled", meeting);
        }
    
        //Todo: Notification is deleted when deleting meeting. This is probably wrong
        appDbContext.Meeting.Remove(meeting);
        await appDbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPatch]
    public async Task<ActionResult<MeetingViewModel>> UpdateMeeting(int meetingId, [FromBody] UpdateMeetingViewModel updateMeeting)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var meeting = appDbContext.Meeting
            .Include(meeting => meeting.Owner)
            .Include(meeting => meeting.Attendees)
            .SingleOrDefault(meeting => meeting.ID == meetingId);
    
        if (meeting == null)
        {
            return NotFound("Meeting with id: " + meetingId + " was not found.");
        }
        //Todo: Check for update permissions. Secretary can edit managers meetings. CEO can edit everything
        if (meeting.Owner.Id != userId) //Todo: More sophisticated permission checking needed
        {
            return Unauthorized("You do not have permissions for this action.");
        }
    
        foreach (var attendee in meeting.Attendees)
        {
            await NotifyUser(attendee, "Meeting changed", "Meeting, which you attend was changed", meeting);
        }
    
        meeting.Title = updateMeeting.Title ?? meeting.Title;
        meeting.Description = updateMeeting.Description ?? meeting.Description;
        meeting.From = updateMeeting.From ?? meeting.From;
        meeting.Until = updateMeeting.Until ?? meeting.Until;
    
        appDbContext.Update(meeting);
        await appDbContext.SaveChangesAsync();
        return meeting.ToViewModel();
    }

    /// <summary>
    /// Gets all relevant meetings for currently logged user
    /// </summary>
    /// <returns>Object with relevant meeting for logged user</returns>
    [HttpGet]
    public GetMeetingViewModel GetMeetings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = appDbContext.Account.SingleOrDefault(u => u.Id == userId);
        var relevantMeetings = appDbContext.Meeting
            .Where(meeting => meeting.Owner == user)
            .Select(meeting => meeting.ToViewModel())
            .ToArray(); //Todo: Add meetings where user is attendee
        return new GetMeetingViewModel
        {
            Meetings = relevantMeetings
        };
    }

    /// <summary>
    /// Creates new meeting in database
    /// </summary>
    /// <param name="meeting">New meeting description</param>
    /// <returns>Created meeting</returns>
    [HttpPost]
    public async Task<ActionResult<MeetingViewModel>> CreateMeeting([FromBody] CreateMeetingViewModel meeting)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // var user = appDbContext.Account.SingleOrDefault(u => u.Id == userId);
        var user = userManager.Users.SingleOrDefault(u => u.Id == userId);
    
        Manager owner;
        switch (user)
        {
            case Secretary secretary:
                owner = secretary.Manager;
                break;
            case Manager manager:
                owner = manager;
                break;
            default:
                return Unauthorized("You do not have permissions for this action");
        }
    
        var attendees = appDbContext.Manager.Where(a => meeting.Attendees.Contains(a.Id)).ToList();
    
        //Todo: Check for attendees and create notification if not empty (Send SignalR message if connected)
        var meetingDb = new Meeting
        {
            Title = meeting.Title,
            Description = meeting.Description,
            From = meeting.From,
            Until = meeting.Until,
            Owner = owner,
            Attendees = attendees
        };
    
        foreach (var manager in attendees)
        {
            await NotifyUser(manager, "Meeting created", "You were added to meeting.", meetingDb);
        }
    
        var dbEntry = appDbContext.Meeting.Add(meetingDb);
        await appDbContext.SaveChangesAsync();
        return dbEntry.Entity.ToViewModel();
    }

    private async Task NotifyUser(Account user, string title, string text, Meeting meeting)
    {
        var recipient = appDbContext.Manager.SingleOrDefault(a => a.Id == user.Id);
        //Todo: Notify connected user with signalR
        var notification = new Notification
        {
            Date = DateTime.Now,
            Meeting = meeting,
            Recipient = recipient,
            Text = text,
            Title = title,
        };


        if (NotificationHub.ConnectedUsers.TryGetValue(user.Id, out var signalRConnectionId))
        {
            await notificationHub.Clients.Client(signalRConnectionId)
                .SendAsync("NotificationAdded", JsonSerializer.Serialize(notification.ToViewModel()));
        }

        appDbContext.Notification.Add(notification);
    }
}
