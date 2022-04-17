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

function setMonth(){
    var month = calendar.getDate().getMonth()+1;
    var year = calendar.getDate().getFullYear();
    document.getElementById('actual_month').innerHTML = year+'/'+month;
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
    useDetailPopup: false,
    // list of Calendars that can be used to add new schedule
    calendars: [
        {
        id: '1',
        name: 'My Calendar',
        }
    ],
});
load_manager_meetings(calendar);
document.getElementById('prev_button').addEventListener("click", calendar.prev());
document.getElementById('next_button').addEventListener("click", calendar.next());
setMonth();


// event handlers
calendar.on({
    'clickSchedule': function(e) {

        console.log('clickSchedule', e);
        showManagerMeeting(e);
    },
    'beforeCreateSchedule': function(e) {
        console.log('beforeCreateSchedule', e);
        // open a creation popup

        // If you dont' want to show any popup, just use `e.guide.clearGuideElement()`

        // then close guide element(blue box from dragging or clicking days)
        e.guide.clearGuideElement();
    },
    //UPRAVIT NA UPDATE MEETING
    'beforeUpdateSchedule': function(e) {
        console.log('beforeUpdateSchedule', e);
        e.schedule.start = e.start;
        e.schedule.end = e.end;
        cal.updateSchedule(e.schedule.id, e.schedule.calendarId, e.schedule);
    },
    'beforeDeleteSchedule': function(e) {
        console.log('beforeDeleteSchedule', e);
        cal.deleteSchedule(e.schedule.id, e.schedule.calendarId);
    }
});

function showManagerMeeting(e){
    document.getElementById('meeting_id_man_view').value  = e.schedule.id;
    document.getElementById('title_man_view').value  = e.schedule.title;
    document.getElementById('start_date_man_view').value  = new Date(e.schedule.start);
    document.getElementById('end_date_man_view').value  = new Date(e.schedule.end);
    document.getElementById('description_man_view').value  = e.schedule.body;
    document.getElementById('modal_view').style.display = "block";
}

function hideManagerMeeting(){
    document.getElementById('meeting_id_man_view').value  = "";
    document.getElementById('title_man_view').value  = "";
    document.getElementById('start_date_man_view').value  = "";
    document.getElementById('end_date_man_view').value  = "";
    document.getElementById('description_man_view').value  = "";
    document.getElementById('modal_view').style.display = "none";
}

function changeDatetimeFormat(input_datetime){
    
    const datetime = new Date(input_datetime);

    //extract the parts of the date
    const day = datetime.getDate();
    const month = datetime.getMonth() + 1;
    const year = datetime.getFullYear();
    
    const hours = datetime.getHours();
    const minutes = datetime.getMinutes();

    let format = year.toString();   
    format = format + "-" + month.toString().padStart(2,"0");   
    format = format + "-" + day.toString().padStart(2,"0"); 

    format = format + " " + hours.toString().padStart(2,"0"); 
    format = format + ":" + minutes.toString().padStart(2,"0");

    return format;
}

function saveManagerMeeting(){

    let name = document.getElementById('title_man_view').value.toString();
    name = name ? name : null

    let description = document.getElementById('description_man_view').value;
    description = description ? description : null

    let from = new Date(document.getElementById('start_date_man_view').value).toISOString();
    from = from ? from : null

    let until = new Date(document.getElementById('end_date_man_view').value).toISOString();
    until = until ? until : null

    let meetingId = document.getElementById('meeting_id_man_view').value;
    console.log(meetingId, name, description, from, until);
    // Calls create meeting endpoint on server
    updateMeeting(meetingId, name, description, from, until).then(response => response.json())
        .then(data => {
           console.log(JSON.stringify(data, null, 2));
           hideManagerMeeting();
           calendar.render();
        })
}
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

