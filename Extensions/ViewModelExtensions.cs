using WAP_PIS.Authorization;
using WAP_PIS.Database;
using WAP_PIS.Models;

namespace WAP_PIS.Extensions;

public static class ViewModelExtensions
{
    public static MeetingViewModel ToViewModel(this Meeting meeting)
    {
        return new MeetingViewModel
        {
            ID = meeting.ID,
            Title = meeting.Title,
            Description = meeting.Description,
            From = meeting.From,
            Until = meeting.Until,
            Owner = meeting.Owner.ToViewModel(),
            Attendees = meeting.Attendees?.Select(m => m.Id)?.ToArray() ?? Array.Empty<string>()
        };
    }

    public static NotificationViewModel ToViewModel(this Notification notification)
    {
        return new NotificationViewModel
        {
            Date = notification.Date,
            Meeting = notification.Meeting.ToViewModel(),
            Text = notification.Text,
            Title = notification.Title,
            ID = notification.ID
        };
    }

    public static AccountViewModel ToAccountViewModel(this Account account)
    {
        return new AccountViewModel
        {
            Id = account.Id,
            Name = account.Name,
            Surname = account.Surname,
            Role = account.GetRole()
        };
    }
    
    public static ManagerViewModel ToViewModel(this Manager manager)
    {
        return new ManagerViewModel
        {
            Id = manager.Id,
            Name = manager.Name,
            Surname = manager.Surname,
            IsCeo = manager.IsCEO
        };
    }

     public static SecretaryViewModel ToViewModel(this Secretary secretary)
    {
        return new SecretaryViewModel
        {
            Id = secretary.Id,
            Name = secretary.Name,
            Surname = secretary.Surname,
            Manager = secretary.Manager.ToViewModel()
        };
    }
}
