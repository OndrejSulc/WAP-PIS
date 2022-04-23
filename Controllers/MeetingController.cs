using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Xml.Schema;
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
        var user = userManager.Users.SingleOrDefault(u => u.Id == userId);

        var meeting = appDbContext.Meeting
            .Include(meeting => meeting.Attendees)
            .Include(meeting => meeting.Owner)
            .SingleOrDefault(meeting => meeting.ID == meetingId);

        if (meeting == null)
        {
            return NotFound("Meeting with id: " + meetingId + " was not found.");
        }

        if (!CheckModificationPermission(user, meeting))
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
        var user = userManager.Users.SingleOrDefault(u => u.Id == userId);

        var meeting = appDbContext.Meeting
            .Include(meeting => meeting.Owner)
            .Include(meeting => meeting.Attendees)
            .SingleOrDefault(meeting => meeting.ID == meetingId);

        if (meeting == null)
        {
            return NotFound("Meeting with id: " + meetingId + " was not found.");
        }

        if (!CheckModificationPermission(user, meeting))
        {
            return Unauthorized("You do not have permissions for this action.");
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
    public ActionResult<GetMeetingViewModel> GetMeetings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = userManager.Users.SingleOrDefault(u => u.Id == userId);
        if (user == null) return NotFound("Logged user not found in database");
        var relevantMeetings = GetMeetingsForAccount(user)
            .Select(meeting => meeting.ToViewModel())
            .ToArray();

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

        var meetingDb = new Meeting
        {
            Title = meeting.Title,
            Description = meeting.Description,
            From = meeting.From,
            Until = meeting.Until,
            Owner = owner,
        };

        if (user is Secretary s)
        {
            await NotifyUser(s.Manager, "Meeting created", "Meeting was created for you by your secretary", meetingDb);
        }

        var dbEntry = appDbContext.Meeting.Add(meetingDb);
        await appDbContext.SaveChangesAsync();
        return dbEntry.Entity.ToViewModel();
    }


    /// <summary>
    /// Returns meetings for user with specified userId
    /// </summary>
    /// <param name="userId">Id of user for meetings search</param>
    /// <returns>Relevant meetings for user with specified user ID</returns>
    [HttpGet]
    [Authorize("CEO")]
    public ActionResult<GetMeetingViewModel> GetMeetingsForUser(string userId)
    {
        var user = userManager.Users.SingleOrDefault(u => u.Id == userId);
        if (user == null) return NotFound($"User with id: {userId} was not found");
        var relevantMeetings = GetMeetingsForAccount(user)
            .Select(meeting => meeting.ToViewModel())
            .ToArray();
        return new GetMeetingViewModel
        {
            Meetings = relevantMeetings
        };
    }

    /// <summary>
    /// Returns all meetings in database
    /// </summary>
    /// <returns>All meetings in system</returns>
    [HttpGet]
    [Authorize("CEO")]
    public ActionResult<GetMeetingViewModel> GetAllMeetings()
    {
        var meetings = appDbContext.Meeting.ToList()
            .Select(meetings => meetings.ToViewModel())
            .ToArray();
        return new GetMeetingViewModel
        {
            Meetings = meetings
        };
    }

    /// <summary>
    /// Creates new group meeting
    /// </summary>
    /// <param name="meeting">New group meeting description</param>
    /// <returns>Created meeting</returns>
    [HttpPost]
    [Authorize("CEO")]
    public async Task<ActionResult<MeetingViewModel>> CreateGroupMeeting([FromBody] CreateGroupMeetingViewModel meeting)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var attendees = userManager.Users.Where(u => meeting.Attendees.Contains(u.Id)).OfType<Manager>().ToList();
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

        var meetingDb = new Meeting
        {
            Title = meeting.Title,
            Description = meeting.Description,
            From = meeting.From,
            Until = meeting.Until,
            Owner = owner,
            Attendees = attendees.ToList()
        };

        if (user is Secretary s)
        {
            await NotifyUser(s.Manager, "Meeting created", "Meeting was created for you by your secretary", meetingDb);
        }

        foreach (var manager in attendees)
        {
            await NotifyUser(manager, "Meeting created", "You were added to group meeting", meetingDb);
        }


        var dbEntry = appDbContext.Meeting.Add(meetingDb);
        await appDbContext.SaveChangesAsync();
        return dbEntry.Entity.ToViewModel();
    }

    [HttpPatch]
    [Authorize("CEO")]
    public async Task<ActionResult<MeetingViewModel>> UpdateGroupMeeting(int meetingId, [FromBody] UpdateGroupMeetingViewModel updateMeeting)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = userManager.Users.SingleOrDefault(u => u.Id == userId);

        List<Manager> attendees;
        if (updateMeeting.Attendees is { Length: > 0 })
        {
            attendees = appDbContext.Manager.Where(m => updateMeeting.Attendees.Contains(m.Id)).ToList();
        }
        else
        {
            attendees = new List<Manager>();
        }

        var meeting = appDbContext.Meeting
            .Include(meeting => meeting.Owner)
            .Include(meeting => meeting.Attendees)
            .SingleOrDefault(meeting => meeting.ID == meetingId);

        if (meeting == null)
        {
            return NotFound("Meeting with id: " + meetingId + " was not found.");
        }



        meeting.Title = updateMeeting.Title ?? meeting.Title;
        meeting.Description = updateMeeting.Description ?? meeting.Description;
        meeting.From = updateMeeting.From ?? meeting.From;
        meeting.Until = updateMeeting.Until ?? meeting.Until;

        foreach (var manager in meeting.Attendees)
        {
            await NotifyUser(manager, "Updated meeting", "Meeting you participate in has been updated.", meeting);
        }

        foreach (var manager in attendees.Except(meeting.Attendees))
        {
            await NotifyUser(manager, "New meeting", "You were added to meeting", meeting);
        }
        meeting.Attendees = updateMeeting.Attendees == null ? meeting.Attendees : attendees;

        appDbContext.Update(meeting);
        await appDbContext.SaveChangesAsync();
        return meeting.ToViewModel();
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

    private bool CheckModificationPermission(Account? user, Meeting meeting)
    {
        switch (user)
        {
            case Secretary secretary:
                return meeting.Owner == secretary.Manager;
            case Manager manager:
                if (manager.IsCEO) return true;
                return meeting.Owner == manager;
            default:
                return false;
        }
    }

    private List<Meeting> GetMeetingsForAccount(Account account)
    {
        var manager = (account is Secretary s ? s.Manager : account as Manager)!;

        return appDbContext.Meeting
            .Include(m => m.Attendees)
            .Where(meeting => meeting.Owner == manager || meeting.Attendees.Contains(manager))
            .ToList();
    }
}
