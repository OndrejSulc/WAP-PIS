function load_manager_meetings(calendar){
    let meetings = getMeetings();
    meetings
        .then(response => response.json())
        .then(data => {
            for(item of data.meetings) {
                calendar.createSchedules([
                    {
                    id: item.id,
                    calendarId: '1',
                    title: item.title,
                    body: item.description,
                    category: 'time',
                    dueDateClass: '',
                    start: new Date(item.from),
                    end: new Date(item.until)
                    }
                ]);
            }
        })
    calendar.render();
    calendar.toggleScheduleView(true);
}

var Calendar = tui.Calendar;
var calendar = new Calendar('#calendar', {
    defaultView: 'month',
    taskView: true,
    scheduleView: true,
    template: {
        monthDayname: function(dayname) {
        return '<span class="calendar-week-dayname-name">' + dayname.label + '</span>';
        }
    },
    month: {
        daynames: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        startDayOfWeek: 0,
        narrowWeekend: true
    },
    useCreationPopup: true,
    // whether use default detail popup or not
    useDetailPopup: true,
    // list of Calendars that can be used to add new schedule
    calendars: [
        {
        id: '1',
        name: 'My Calendar',
        }
    ],
});
load_manager_meetings(calendar);

//console.log(exampleMeetings.meetings);
/*
calendar.createSchedules([
    {
    id: '1',
    calendarId: '1',
    title: 'my schedule',
    category: 'time',
    dueDateClass: '',
    start: new Date('2022-04-18T07:30:00'),
    end: new Date('2022-04-18T09:30:00')
    }
]);
for(item of exampleMeetings.meetings) {
    calendar.createSchedules([
        {
        id: item.ID,
        calendarId: '1',
        title: item.Title.toString(),
        category: 'time',
        dueDateClass: '',
        start: new Date(item.From),
        end: new Date(item.Until)
        }
    ]);
    console.log(item.ID,item.Title,item.From,item.Until);
}*/

