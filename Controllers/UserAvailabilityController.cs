using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAP_PIS.Database;
using WAP_PIS.Models;

namespace WAP_PIS.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserAvailabilityController : ControllerBase
{
    private ILogger<UserAvailabilityController> logger;
    private ApplicationDbContext dbContext;

    public UserAvailabilityController(ILogger<UserAvailabilityController> logger, ApplicationDbContext dbContext)
    {
        this.logger = logger;
        this.dbContext = dbContext;
    }
    
    [HttpGet]
    [Authorize("CEO")]
    public ActionResult<UserAvailabilityResultViewModel> IsAvailable(string user, DateTime from, DateTime to, int? ignoreMeeting = null)
    {
        if (dbContext.Account.SingleOrDefault(account => account.Id == user) == null) //Check if user with specified id exists
        {
            return NotFound($"User with id {user} not found.");
        }
        
        var overlappingMeetings = dbContext.Meeting
            .Include(meeting => meeting.Attendees)
            .Where(meeting => meeting.Owner.Id == user || meeting.Attendees.Select(manager => manager.Id).Contains(user))
            .Where(meeting => meeting.ID != ignoreMeeting)
            .Where(meeting => meeting.From < to && from < meeting.Until);
        return new UserAvailabilityResultViewModel
        {
            Available = !overlappingMeetings.Any()
        };
    }
    
    
    [HttpGet]
    public ActionResult<UserAvailabilityResultViewModel> IsCeoAvailable(DateTime from, DateTime to)
    {
        var ceo = dbContext.Manager
            .Single(manager => manager.IsCEO).Id;
    
        var overlappingMeetings = dbContext.Meeting
            .Include(meeting => meeting.Attendees)
            .Where(meeting => meeting.Owner.Id == ceo || meeting.Attendees.Select(manager => manager.Id).Contains(ceo))
            .Where(meeting => meeting.From < to && from < meeting.Until);
        return new UserAvailabilityResultViewModel
        {
            Available = !overlappingMeetings.Any()
        };
    }
}
