using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAP_PIS.Models;
using WAP_PIS.Database;
using WAP_PIS.Extensions;

namespace WAP_PIS.Controllers;

/// <summary>
/// Controller for managing meetings
/// </summary>
public class MeetingController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext appDbContext;

    public MeetingController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext)
    {
        _logger = logger;
        appDbContext = applicationDbContext;
    }

    [HttpDelete]
    public IActionResult DeleteMeeting(int meetingId)
    {
        var userId = 0; //Todo: Get logged in user from identity framework
        var meeting = appDbContext.Meeting
            .Include(meeting => meeting.Attendees)
            .SingleOrDefault(meeting => meeting.ID == meetingId);
        if (meeting == null)
        {
            return NotFound("Meeting with id: " + meetingId + " was not found.");
        }
        //Todo: Check for update permissions. Secretary can remove managers meetings. CEO can edit everything
        if (meeting.Owner != userId) //Todo: More sophisticated permission checking needed
        {
            return Unauthorized("You do not have permissions for this action.");
        }

        foreach (var attendee in meeting.Attendees)
        {
            NotifyUser(attendee, "Meeting cancelled", "Meeting, which you attend was cancelled", meeting);
        }

        //Todo: Notification is deleted when deleting meeting. This is probably wrong
        appDbContext.Meeting.Remove(meeting);
        appDbContext.SaveChanges();
        return Ok();
    }

    [HttpPatch]
    public ActionResult<MeetingViewModel> UpdateMeeting(int meetingId, [FromBody] UpdateMeetingViewModel updateMeeting)
    {
        var userId = 0; //Todo: Get logged in user from identity framework
        var meeting = appDbContext.Meeting
            .Include(meeting => meeting.Attendees)
            .SingleOrDefault(meeting => meeting.ID == meetingId);

        if (meeting == null)
        {
            return NotFound("Meeting with id: " + meetingId + " was not found.");
        }
        //Todo: Check for update permissions. Secretary can edit managers meetings. CEO can edit everything
        if (meeting.Owner != userId) //Todo: More sophisticated permission checking needed
        {
            return Unauthorized("You do not have permissions for this action.");
        }

        foreach (var attendee in meeting.Attendees)
        {
            NotifyUser(attendee, "Meeting changed", "Meeting, which you attend was changed", meeting);
        }

        meeting.Title = updateMeeting.Title ?? meeting.Title;
        meeting.Description = updateMeeting.Description ?? meeting.Description;
        meeting.From = updateMeeting.From ?? meeting.From;
        meeting.Until = updateMeeting.Until ?? meeting.Until;

        appDbContext.Update(meeting);
        appDbContext.SaveChanges();
        return meeting.ToViewModel();
    }

    /// <summary>
    /// Gets all relevant meetings for currently logged user
    /// </summary>
    /// <returns>Object with relevant meeting for logged user</returns>
    [HttpGet]
    public GetMeetingViewModel GetMeetings()
    {
        var user = 0; //Todo: Get user from identity framework
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
    public MeetingViewModel CreateMeeting([FromBody] CreateMeetingViewModel meeting)
    {
        var userId = 0; //Todo: Add owner id from identity framework

        var attendees = appDbContext.Account.Where(account => meeting.Attendees.Contains(account.ID)).ToList();

        //Todo: Check for attendees and create notification if not empty (Send SignalR message if connected)
        var meetingDb = new Meeting
        {
            Title = meeting.Title,
            Description = meeting.Description,
            From = meeting.From,
            Until = meeting.Until,
            Owner = userId,
            Attendees = attendees
        };

        foreach (var attendee in attendees)
        {
            NotifyUser(attendee, "Meeting created", "You were added to meeting.", meetingDb);
        }

        var dbEntry = appDbContext.Meeting.Add(meetingDb);
        appDbContext.SaveChanges();
        return dbEntry.Entity.ToViewModel();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }



    private void NotifyUser(Account user, string title, string text, Meeting meeting)
    {
        //Todo: Notify connected user with signalR
        var notification = new Notification
        {
            Date = DateTime.Now,
            Meeting = meeting,
            Recipient = user,
            Text = text,
            Title = title,
        };
        appDbContext.Notification.Add(notification);
    }
}
