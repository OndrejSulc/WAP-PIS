using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WAP_PIS.Models;
using WAP_PIS.Database;

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
        var meeting = appDbContext.Meeting.SingleOrDefault(meeting => meeting.ID == meetingId);
        if (meeting == null)
        {
            return NotFound("Meeting with id: " + meetingId + " was not found.");
        }
        //Todo: Check for update permissions. Secretary can remove managers meetings. CEO can edit everything
        if (meeting.Owner != userId) //Todo: More sophisticated permission checking needed
        {
            return Unauthorized("You do not have permissions for this action.");
        }
        appDbContext.Meeting.Remove(meeting);
        appDbContext.SaveChanges();
        return Ok();
    }

    [HttpPatch]
    public ActionResult<MeetingViewModel> UpdateMeeting(int meetingId, [FromBody] UpdateMeetingViewModel updateMeeting)
    {
        var userId = 0; //Todo: Get logged in user from identity framework
        var meeting = appDbContext.Meeting.SingleOrDefault(meeting => meeting.ID == meetingId);
        if (meeting == null)
        {
            return NotFound("Meeting with id: " + meetingId + " was not found.");
        }
        //Todo: Check for update permissions. Secretary can edit managers meetings. CEO can edit everything
        if (meeting.Owner != userId) //Todo: More sophisticated permission checking needed
        {
            return Unauthorized("You do not have permissions for this action.");
        }

        meeting.Title = updateMeeting.Title ?? meeting.Title;
        meeting.Description = updateMeeting.Description ?? meeting.Description;
        meeting.From = updateMeeting.From ?? meeting.From;
        meeting.Until = updateMeeting.Until ?? meeting.Until;
        appDbContext.Update(meeting);
        appDbContext.SaveChanges();
        return new MeetingViewModel
        {
            Title = meeting.Title,
            Description = meeting.Description,
            From = meeting.From,
            Until = meeting.Until,
            Owner = meeting.Owner,
            ID = meeting.ID
        };
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
            .Select(meeting => new MeetingViewModel
            {
                ID = meeting.ID,
                Title = meeting.Title,
                Description = meeting.Description,
                From = meeting.From,
                Until = meeting.Until,
                Owner = meeting.Owner
            })
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
    public Meeting CreateMeeting([FromBody] CreateMeetingViewModel meeting)
    {
        //Todo: Check for attendees and create notification if not empty (Send SignalR message if connected)
        var meetingDb = new Meeting
        {
            Title = meeting.Title,
            Description = meeting.Description,
            From = meeting.From,
            Until = meeting.Until,
            Owner = 0 //Todo: Add owner id from identity framework
        };
        var dbEntry = appDbContext.Meeting.Add(meetingDb);
        appDbContext.SaveChanges();
        return dbEntry.Entity;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
