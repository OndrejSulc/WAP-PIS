using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAP_PIS.Database;
using WAP_PIS.Models;

namespace WAP_PIS.Controllers;

public class UserAvailabilityController : Controller
{
    private ILogger<UserAvailabilityController> logger;
    private ApplicationDbContext dbContext;

    public UserAvailabilityController(ILogger<UserAvailabilityController> logger, ApplicationDbContext dbContext)
    {
        this.logger = logger;
        this.dbContext = dbContext;
    }

    [HttpGet]
    public ActionResult<UserAvailabilityResultViewModel> IsAvailable(int user, DateTime from, DateTime to)
    {
        if (dbContext.Account.SingleOrDefault(account => account.ID == user) == null) //Check if user with specified id exists
        {
            return NotFound($"User with id {user} not found.");
        }

        var overlappingMeetings = dbContext.Meeting
            .Include(meeting => meeting.Attendees)
            .Where(meeting => meeting.Owner == user || meeting.Attendees.Select(account => account.ID).Contains(user))
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
            .Include(manager => manager.Account)
            .Single(manager => manager.IsCEO).Account.ID;

        var overlappingMeetings = dbContext.Meeting
            .Include(meeting => meeting.Attendees)
            .Where(meeting => meeting.Owner == ceo || meeting.Attendees.Select(account => account.ID).Contains(ceo))
            .Where(meeting => meeting.From < to && from < meeting.Until);
        return new UserAvailabilityResultViewModel
        {
            Available = !overlappingMeetings.Any()
        };
    }
}
