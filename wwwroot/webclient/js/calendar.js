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
                    attendees: item.attendees,
                    category: 'time',
                    dueDateClass: '',
                    start: new Date(item.from),
                    end: new Date(item.until)
                    }
                ]);
            }
        });
    calendar.render();
    calendar.toggleScheduleView(true);
}



// Option 2 - jQuery Smooth Scrolling
$('a.nav-li-a').on('click', function (e) {
    if (this.hash !== '') {
        e.preventDefault();
        console.log(this.hash);

        const hash = this.hash;
        var targetOffset = $(this.hash).offset().top;
        console.log(targetOffset);

        $("html, body").animate({
            scrollTop: targetOffset + "px"
        }, 1000);
    }
});

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
    useCreationPopup: false,
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
        startCreateManagerMeeting(e);
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

function hideManagerMeeting(){
    document.getElementById('calendar_id_man_view').value  = "";
    document.getElementById('meeting_id_man_view').value  = "";
    document.getElementById('title_man_view').value  = "";
    document.getElementById('start_date_man_view').value  = "";
    document.getElementById('end_date_man_view').value  = "";
    document.getElementById('description_man_view').value  = "";
    var select = document.getElementById("group_meeting_attendees");
    var length = select.length;
    for (i = 0; i < length; i++) {
        select.remove(0);
    }
    document.getElementById('group_meeting_man_view').checked = false; 
    document.getElementById('group_meeting_attendees').style.display = "none";
    document.getElementById('modal_view').style.display = "none";
}

function showManagerMeeting(e){
    document.getElementById('calendar_id_man_view').value  = e.schedule.calendarId;
    document.getElementById('meeting_id_man_view').value  = e.schedule.id;
    document.getElementById('title_man_view').value  = e.schedule.title;
    document.getElementById('start_date_man_view').value  = changeDatetimeFormat(e.schedule.start);
    document.getElementById('end_date_man_view').value  = changeDatetimeFormat(e.schedule.end);
    document.getElementById('description_man_view').value  = e.schedule.body;
    if(e.schedule.attendees.length !== 0){
        document.getElementById('group_meeting_man_view').checked = true; 
        //PRIDAT ATTENDEES
        getAllUsers().then(data => {
            for(item of data.managers) {
                var opt = document.createElement('option');
                opt.value = item.id;
                opt.innerHTML = item.name + " " + item.surname;
                if(e.schedule.attendees.includes(item.id)){
                    opt.selected = true;
                }
                document.getElementById('group_meeting_attendees').appendChild(opt);
            }
        });

            //getUserInfo(e.schedule.attendees[i]).then(data => {
                //console.log(data);
                /*var opt = document.createElement('option');
                opt.value = data.id;
                opt.innerHTML = data.name + " " + data.surname;
                document.getElementById('group_meeting_attendees').appendChild(opt);*/
                /*for(item of data.managers) {
                    var opt = document.createElement('option');
                    opt.value = item.id;
                    opt.innerHTML = item.name + " " + item.surname;
                    document.getElementById('group_meeting_attendees').appendChild(opt);
                }*/
            //});
        document.getElementById('group_meeting_attendees').style.display = "block";
    }


    document.getElementById('modal_view').style.display = "block";
    document.getElementById('man_meeting_view').style.display = "block";
    document.getElementById('man_meeting_create').style.display = "none";
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
    format = format + "T" + hours.toString().padStart(2,"0"); 
    format = format + ":" + minutes.toString().padStart(2,"0");

    return format;
}

function saveManagerMeeting(){

    let name = document.getElementById('title_man_view').value.toString();
    name = name ? name : null

    let description = document.getElementById('description_man_view').value;
    description = description ? description : null

    let from = document.getElementById('start_date_man_view').value;
    from = from ? from : null

    let until = document.getElementById('end_date_man_view').value;
    until = until ? until : null

    let meetingId = document.getElementById('meeting_id_man_view').value;
    let calendarId = document.getElementById('calendar_id_man_view').value;
    console.log(meetingId, name, description, from, until);
    // Calls create meeting endpoint on server
    updateMeeting(meetingId, name, description, from, until).then(response => response.json())
        .then(data => {
            console.log(JSON.stringify(data, null, 2));
            calendar.updateSchedule(parseInt(meetingId), calendarId, {
                title: name,
                body: description,
                start: new Date(document.getElementById('start_date_man_view').value),
                end: new Date(document.getElementById('end_date_man_view').value)
            });
            calendar.render();
            hideManagerMeeting();
        });
}

function deleteManagerMeeting(){
    let meetingId = document.getElementById('meeting_id_man_view').value;
    let calendarId = document.getElementById('calendar_id_man_view').value;
    //Calls remove meeting endpoint
    removeMeeting(meetingId).then(result => {
        calendar.deleteSchedule(parseInt(meetingId), calendarId);
        calendar.render();
        hideManagerMeeting();
    });
}

function startCreateManagerMeeting(e){
    document.getElementById('start_date_man_view').value  = changeDatetimeFormat(e.start);
    document.getElementById('end_date_man_view').value  = changeDatetimeFormat(e.end);
    document.getElementById('modal_view').style.display = "block";
    document.getElementById('man_meeting_view').style.display = "none";
    document.getElementById('man_meeting_create').style.display = "block";
}

function createManagerMeeting(e){

    let name = document.getElementById('title_man_view').value.toString();
    name = name ? name : null

    let description = document.getElementById('description_man_view').value;
    description = description ? description : null

    let from = new Date(document.getElementById('start_date_man_view').value).toISOString();
    from = from ? from : null

    let until = new Date(document.getElementById('end_date_man_view').value).toISOString();
    until = until ? until : null

    //console.log(name,description,from,until);

    if(document.getElementById('group_meeting_man_view').checked == true){
        var attendees = [];
        for (var option of document.getElementById('group_meeting_attendees').options)
        {
            if (option.selected) {
                attendees.push(option.value);
            }
        }
        console.log(attendees);
        createGroupMeeting(name, description, from, until, attendees).then(response => response.json())
            .then(data => {
                let result = JSON.stringify(data, null, 2);
                let item = JSON.parse(result);
                calendar.createSchedules([
                    {
                    id: item.id,
                    calendarId: '1',
                    title: item.title,
                    body: item.description,
                    attendees: item.attendees,
                    category: 'time',
                    dueDateClass: '',
                    start: new Date(item.from),
                    end: new Date(item.until)
                    }
                ]);
                calendar.render();
                hideManagerMeeting();
            });
    }
    else{
        createMeeting(name, description, from, until).then(response => response.json())
            .then(data => {
                let result = JSON.stringify(data, null, 2);
                let item = JSON.parse(result);
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
                calendar.render();
                hideManagerMeeting();
            });
    }
    

    //let meetingId = document.getElementById('meeting_id_man_view').value;
    //let calendarId = document.getElementById('calendar_id_man_view').value;

    /*calendar.createSchedules([
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
    ]);*/
}

function checkGroupMeeting(){
    var group_meeting_checkbox = document.getElementById('group_meeting_man_view');

    if(group_meeting_checkbox.checked === true) {
        console.log("Checkbox is checked - boolean value: ", group_meeting_checkbox.checked);

        getAllUsers().then(data => {
            for(item of data.managers) {
                var opt = document.createElement('option');
                opt.value = item.id;
                opt.innerHTML = item.name + " " + item.surname;
                document.getElementById('group_meeting_attendees').appendChild(opt);
            }
        });

        document.getElementById('group_meeting_attendees').style.display = "block";
    }
        /*for (var i = min; i<=max; i++){
            var opt = document.createElement('option');
            opt.value = i;
            opt.innerHTML = i;
            group_meeting_checkbox.appendChild(opt);
        }*/
    if(group_meeting_checkbox.checked === false) {
        console.log("Checkbox is not checked - boolean value: ", group_meeting_checkbox.checked)
        document.getElementById('group_meeting_attendees').style.display = "none";
    }
}

function load_users() {
    var i = 0;
    getAllUsers().then(data => {
        var table = document.getElementById("user_management");
        table.innerHTML = '';
        for (item of data.managers) {
            var row = table.insertRow(i);

            // Insert new cells (<td> elements) at the 1st and 2nd position of the "new" <tr> element:
            var cell1 = row.insertCell(0);
            var cell2 = row.insertCell(1);
            var cell3 = row.insertCell(2);
            var cell4 = row.insertCell(3);
            //console.log(item.isCeo);
            // Add some text to the new cells:
            if (item.isCeo == true) {
                cell1.innerHTML = "Ceo";
            }
            else {
                cell1.innerHTML = "Manager";
            }
            cell2.innerHTML = item.name;
            cell3.innerHTML = item.surname;
            cell4.innerHTML

                = "<button onclick=\"delete_user('" + item.id + "')\"> Delete!</button>";
            i = i + 1;
        }
        for (item of data.secretaries) {
            var row = table.insertRow(i);

            // Insert new cells (<td> elements) at the 1st and 2nd position of the "new" <tr> element:
            var cell1 = row.insertCell(0);
            var cell2 = row.insertCell(1);
            var cell3 = row.insertCell(2);
            var cell4 = row.insertCell(3);
            //console.log(item.isCeo);
            // Add some text to the new cells:
            if (item.manager.isCeo == true) {
                cell1.innerHTML = "CeoSecreteary";
            }
            else {
                cell1.innerHTML = "Secreteary";
            }
            cell2.innerHTML = item.name;
            cell3.innerHTML = item.surname;
            cell4.innerHTML = "<button onclick=\"delete_user('" + item.id + "')\"> Delete!</button>";
            i = i + 1;
        }
    });
}

function delete_user(id) {
    console.log(deleteUser(id));
    load_users();
}

//const scroll = new SmoothScroll('.navbar a[href*="#"]', {
//    speed: 500
//});

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

