async function load_manager_meetings(calendar){
    let meetings = getMeetings();
    meetings
        .then(response => response.json())
        .then(data => {
            //console.log(data);
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
                    end: new Date(item.until),
                    raw: item.owner.id + "," + item.owner.name + " " + item.owner.surname
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

+$('a.navbar-href').on('click', function (e) {
    if (this.hash !== '') {
        e.preventDefault();
        console.log(this.hash);

        const hash = this.hash;
        var targetOffset = $(this.hash).offset().top;
        console.log(targetOffset);

        $("html, body").animate({
            scrollTop: targetOffset - 80
        }, 1000);
    }
});

/* When the user clicks on the button, 
toggle between hiding and showing the dropdown content */
function closeMenu() {
    document.getElementById("menu-toggle").checked = false;
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
document.getElementById('prev_button').addEventListener("click", calendar.prev());
document.getElementById('next_button').addEventListener("click", calendar.next());
setMonth();
startSignalRNotifications();
get_notifications();

let actual_meeting;
// event handlers
calendar.on({
    'clickSchedule': function(e) {
        //console.log('clickSchedule', e);
        showViewMeeting(e);
        actual_meeting = e;
    }
});

function showCalendarforUser(logged_user_id){
    let actual_user_id = document.getElementById('calendar_for_user').value;
    calendar.clear();
    let meetings = getMeetingsForUser(actual_user_id);
    meetings
        .then(response => response.json())
        .then(data => {
            //console.log(data);
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
                    end: new Date(item.until),
                    raw: item.owner.id + "," + item.owner.name + " " + item.owner.surname
                    }
                ]);
            }
        });
    calendar.render();
    calendar.toggleScheduleView(true);
    if(logged_user_id == actual_user_id){
        document.getElementById('control_buttons').style.display = "block";
        calendar.on({
            'beforeCreateSchedule': function(e) {
                //console.log('beforeCreateSchedule', e);
                startCreateManagerMeeting(e);
                e.guide.clearGuideElement();
            }
        });
    }
    else{
        document.getElementById('control_buttons').style.display = "none";
        calendar.on({
            'beforeCreateSchedule': function(e) {
                //console.log('beforeCreateSchedule', e);
                hideEditMeeting();
                e.guide.clearGuideElement();
            }
        });
    }
}

function hideViewMeeting(){
    document.getElementById('calendar_id_man_view').value  = "";
    document.getElementById('meeting_id_man_view').value  = "";
    document.getElementById('owner_man_view').innerHTML  = "";
    document.getElementById('title_man_view').innerHTML  = "";
    document.getElementById('date_time_man_view').innerHTML  = "";
    document.getElementById('attendees_man_view').innerHTML  = "";
    document.getElementById('description_man_view').innerHTML  = "";
    document.getElementById('modal_view').style.display = "none";
}

function showViewMeeting(e){
    //console.log(e);
    document.getElementById('calendar_id_man_view').value  = e.schedule.calendarId;
    document.getElementById('meeting_id_man_view').value  = e.schedule.id;
    var user = e.schedule.raw.split(",");
    document.getElementById('owner_man_view').innerHTML  = user[1];
    document.getElementById('title_man_view').innerHTML  = e.schedule.title;
    var start_datetime = changeDatetimeFormat(e.schedule.start);
    document.getElementById('date_time_man_view').innerHTML  = start_datetime.replace("T", " ") + " - " + getTimeFromDatetime(e.schedule.end);
    document.getElementById('description_man_view').innerHTML  = e.schedule.body;
    if(e.schedule.attendees.length !== 0){
        var logged = document.getElementById('logged_user_role').value;
        if(logged === "Ceo" || logged === "CeoSecretary"){
            let managers = [];
            getAllUsers().then(data => {
            
                for(item of data.managers) {
                    if(e.schedule.attendees.includes(item.id)){
                        var manager = item.name + " " + item.surname;
                        //console.log("ONE MAN:" +manager);
                        managers.push(manager);
                        //console.log("ONE MAN:" +manager);
                    }
                }
                //console.log("Managers"+managers);
                document.getElementById('attendees_man_view').innerHTML = managers.join(', ');
            });
        }
        else{
            document.getElementById('attendees_man_view').innerHTML = e.schedule.attendees.length;
        }
        document.getElementById('attendees_div_man_view').style.display = "block";        
    }
    else{

    }
    document.getElementById('modal_view').style.display = "block";
}

function hideEditMeeting(){
    document.getElementById('calendar_id_man_edit').value  = "";
    document.getElementById('meeting_id_man_edit').value  = "";
    document.getElementById('title_man_edit').value  = "";
    document.getElementById('start_date_man_edit').value  = "";
    document.getElementById('end_date_man_edit').value  = "";
    document.getElementById('description_man_edit').value  = "";
    document.getElementById('group_meeting_man_edit').checked = false;
    document.getElementById('group_meeting_attendees').innerHTML = '';
    document.getElementById('modal_edit').style.display = "none";
}


function showEditMeeting(){
    hideViewMeeting();
    document.getElementById('calendar_id_man_edit').value  = actual_meeting.schedule.calendarId;
    document.getElementById('meeting_id_man_edit').value  = actual_meeting.schedule.id;
    document.getElementById('title_man_edit').value  = actual_meeting.schedule.title;
    document.getElementById('start_date_man_edit').value  = changeDatetimeFormat(actual_meeting.schedule.start);
    document.getElementById('end_date_man_edit').value  = changeDatetimeFormat(actual_meeting.schedule.end);
    document.getElementById('description_man_edit').value  = actual_meeting.schedule.body;
    if(actual_meeting.schedule.attendees.length !== 0){
        document.getElementById('group_meeting_man_edit').checked = true; 
        getAllUsers().then(data => {
            for(item of data.managers) {
                var opt = document.createElement('option');
                opt.value = item.id;
                opt.innerHTML = item.name + " " + item.surname;
                if(actual_meeting.schedule.attendees.includes(item.id)){
                    opt.selected = true;
                }
                document.getElementById('group_meeting_attendees').appendChild(opt);
            }
        });
        document.getElementById('group_meeting_attendees').style.display = "block";
    }
    document.getElementById('create_new').style.display = "none";
    document.getElementById('save_edit').style.display = "block";
    document.getElementById('modal_edit').style.display = "block";
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

function getTimeFromDatetime(input_datetime){
    
    const datetime = new Date(input_datetime);
    
    const hours = datetime.getHours();
    const minutes = datetime.getMinutes();

    let format = hours.toString().padStart(2,"0"); 
    format = format + ":" + minutes.toString().padStart(2,"0");

    return format;
}

async function saveEditMeeting(){

    let name = document.getElementById('title_man_edit').value;
    name = name ? name : null
    if(name==null){
        alert("Enter the title!");
        return;
    }

    let description = document.getElementById('description_man_edit').value;
    description = description ? description : null
    if(description==null){
        alert("Enter the description!");
        return;
    }

    //console.log("FROM: " + document.getElementById('start_date_man_edit').value);
    let from = document.getElementById('start_date_man_edit').value;
    from = from ? from : null
    if(from==null){
        alert("Enter the start!");
        return;
    }

    let until = document.getElementById('end_date_man_edit').value;
    until = until ? until : null
    if(until==null){
        alert("Enter the end!");
        return;
    }

    let meetingId = document.getElementById('meeting_id_man_edit').value;
    let calendarId = document.getElementById('calendar_id_man_edit').value;

    //console.log(meetingId, name, description, from, until);

    if(document.getElementById('group_meeting_man_edit').checked == true){
        var attendees = [];
        var attendees_names = [];
        var attendees_not_available = [];
        for (var option of document.getElementById('group_meeting_attendees').options)
        {
            if (option.selected) {
                attendees.push(option.value);
                attendees_names.push(option.innerHTML);
            }
        }
        if(attendees.length==0){
            alert("This is group meeting, you must choose at least 1 person!");
            return;
        }
        for(let i = 0; i < attendees.length; i++){ 
            const result = await isUserAvailable(attendees[i],from,until).then(response => response.json())
            .then(data => {
                //console.log(attendees_names[i] + " is: " + data.available + " time: "+ from + " - " + until);
                if(data.available == false)attendees_not_available.push(attendees_names[i]);
            });
        } 
        //console.log("NOT AVAILABLE" + attendees_not_available);

        if(attendees_not_available.length == 0){
            console.log("All Attendees:"+attendees);
            updateGroupMeeting(meetingId, name, description, from, until, attendees).then(response => response.json())
                .then(data => {
                    let result = JSON.stringify(data, null, 2);
                    let item = JSON.parse(result);
                    calendar.updateSchedule(parseInt(meetingId), calendarId, {
                        title: name,
                        body: description,
                        attendees: attendees,
                        start: new Date(from),
                        end: new Date(until)
                    });
                    calendar.render();
                    hideEditMeeting();
                });
        }
        else{
            alert("These users are not available: "+attendees_not_available.join(", "));
        }
    }
    else{
        var logged = document.getElementById('logged_user_role').value;
        if(logged === "Manager" || logged === "Secretary"){
            isCeoAvailable(from, until).then(response => response.json())
            .then(data => {
                console.log(data.available);
                if(data.available == false){
                    alert("CEO is not available!");
                }
                else{
                    updateMeeting(meetingId, name, description, from, until).then(response => response.json())
                    .then(data => {
                        console.log(JSON.stringify(data, null, 2));
                        calendar.updateSchedule(parseInt(meetingId), calendarId, {
                            title: name,
                            body: description,
                            attendees:[],
                            start: new Date(from),
                            end: new Date(until)
                        });
                        calendar.render();
                        hideEditMeeting();
                    });
                }
            });
        }
        else{
            updateMeeting(meetingId, name, description, from, until).then(response => response.json())
            .then(data => {
                console.log(JSON.stringify(data, null, 2));
                calendar.updateSchedule(parseInt(meetingId), calendarId, {
                    title: name,
                    body: description,
                    attendees:[],
                    start: new Date(from),
                    end: new Date(until)
                });
                calendar.render();
                hideEditMeeting();
            });
        }
    }
}

function deleteMeeting(){
    let meetingId = document.getElementById('meeting_id_man_view').value;
    let calendarId = document.getElementById('calendar_id_man_view').value;
    //Calls remove meeting endpoint
    removeMeeting(meetingId).then(result => {
        calendar.deleteSchedule(parseInt(meetingId), calendarId);
        calendar.render();
        hideViewMeeting(); 
    });
}

function startCreateManagerMeeting(e){
    document.getElementById('start_date_man_edit').value  = changeDatetimeFormat(e.start);
    document.getElementById('end_date_man_edit').value  = changeDatetimeFormat(e.end);
    document.getElementById('create_new').style.display = "block";
    document.getElementById('save_edit').style.display = "none";
    document.getElementById('modal_edit').style.display = "block";
}

async function create_Meeting(){

    let name = document.getElementById('title_man_edit').value;
    name = name ? name : null
    if(name==null){
        alert("Enter the title!");
        return;
    }

    let description = document.getElementById('description_man_edit').value;
    description = description ? description : null
    if(description==null){
        alert("Enter the description!");
        return;
    }

    //console.log("FROM: " + document.getElementById('start_date_man_edit').value);
    let from = document.getElementById('start_date_man_edit').value;
    from = from ? from : null
    if(from==null){
        alert("Enter the start!");
        return;
    }

    let until = document.getElementById('end_date_man_edit').value;
    until = until ? until : null
    if(until==null){
        alert("Enter the end!");
        return;
    }

    console.log(name,description,from,until);

    if(document.getElementById('group_meeting_man_edit').checked == true){
        var attendees = [];
        var attendees_names = [];
        var attendees_not_available = [];
        for (var option of document.getElementById('group_meeting_attendees').options)
        {
            if (option.selected) {
                attendees.push(option.value);
                attendees_names.push(option.innerHTML);
            }
        }
        if(attendees.length==0){
            alert("This is group meeting, you must choose at least 1 person!");
            return;
        }
        for(let i = 0; i < attendees.length; i++){ 
            const result = await isUserAvailable(attendees[i],from,until).then(response => response.json())
            .then(data => {
                //console.log(attendees_names[i] + " is: " + data.available + " time: "+ from + " - " + until);
                if(data.available == false)attendees_not_available.push(attendees_names[i]);
            });
        } 
        //console.log("NOT AVAILABLE" + attendees_not_available);

        if(attendees_not_available.length == 0){
            console.log("All Attendees:"+attendees);
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
                        end: new Date(item.until),
                        raw: item.owner.id + "," + item.owner.name + " " + item.owner.surname
                        }
                    ]);
                    calendar.render();
                    hideEditMeeting();
                });
        }
        else{
            alert("These users are not available: "+attendees_not_available.join(", "));
        }
    }
    else{
        var logged = document.getElementById('logged_user_role').value;
        if(logged === "Manager" || logged === "Secretary"){
            isCeoAvailable(from, until).then(response => response.json())
            .then(data => {
                console.log(data.available);
                if(data.available == false){
                    alert("CEO is not available!");
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
                                start: item.from,
                                end: item.until,
                                raw: item.owner.id + "," + item.owner.name + " " + item.owner.surname
                                }
                            ]);
                            calendar.render();
                            hideEditMeeting();
                        });
                }
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
                        start: item.from,
                        end: item.until,
                        raw: item.owner.id + "," + item.owner.name + " " + item.owner.surname
                        }
                    ]);
                    calendar.render();
                    hideEditMeeting();
                });
        }
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
    var group_meeting_checkbox = document.getElementById('group_meeting_man_edit');

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
    if(group_meeting_checkbox.checked === false) {
        console.log("Checkbox is not checked - boolean value: ", group_meeting_checkbox.checked)
        document.getElementById('group_meeting_attendees').style.display = "none";
        var select = document.getElementById("group_meeting_attendees");
        var length = select.length;
        for (i = 0; i < length; i++) {
            select.remove(0);
        }
    }
}

//     USER MANAGEMENT
function fill_table_of_users(list,type){
    var table = document.getElementById("user_management");
    var i = table.rows.length;

    for (item of list) {
        var row = table.insertRow(i);

        // Insert new cells (<td> elements) at the 1st and 2nd position of the "new" <tr> element:
        var role = row.insertCell(0);
        var name = row.insertCell(1);
        var surname = row.insertCell(2);
        //var edit = row.insertCell(3);
        var delete_button = row.insertCell(3);
        //console.log(item.isCeo);
        // Add some text to the new cells:
        if(type === "managers"){
            if (item.isCeo == true) {
                role.innerHTML = "Ceo";
            }
            else {
                role.innerHTML = "Manager";
            }
        }
        else{
            if (item.manager.isCeo == true) {
                role.innerHTML = "CeoSecreteary";
            }
            else {
                role.innerHTML = "Secreteary";
            }
        }
        name.innerHTML = item.name;
        surname.innerHTML = item.surname;
        //edit.innerHTML = "<button onclick=\"showUser('"+ item.id +"');\"> Edit!</button>";
        delete_button.innerHTML = "<button class=\"btn\" onclick=\"delete_user('" + item.id + "')\"> Delete!</button>";
        i = i + 1;
    }
}

function load_users() {
    var i = 0;
    getAllUsers().then(data => {
        var table = document.getElementById("user_management");
        table.innerHTML = '';
        fill_table_of_users(data.managers,"managers");
        fill_table_of_users(data.secretaries,"secretaries");
    });
}

function delete_user(id) {
    console.log(deleteUser(id));
    setTimeout(function(){
        load_users();
      }, 500);
}

function createNewUser(){

    document.getElementById('um_login').value  = "";
    document.getElementById('um_password').value  = "";
    document.getElementById('um_name').value  = "";
    document.getElementById('um_surname').value  = "";
    document.getElementById('um_date_of_birth').value  = "";
    var select = document.getElementById("secretary_attendees");
    var length = select.length;
    for (i = 0; i < length; i++) {
        select.remove(0);
    }
    document.getElementById('um_issecretary').checked = false; 
    document.getElementById('secretary_attendees').style.display = "none";
    document.getElementById('create_user_view').style.display = "block";
}

function hideUser(){
    document.getElementById('um_login').value  = "";
    document.getElementById('um_password').value  = "";
    document.getElementById('um_name').value  = "";
    document.getElementById('um_surname').value  = "";
    document.getElementById('um_date_of_birth').value  = "";
    var select = document.getElementById("secretary_attendees");
    var length = select.length;
    for (i = 0; i < length; i++) {
        select.remove(0);
    }
    document.getElementById('um_issecretary').checked = false; 
    document.getElementById('secretary_attendees').style.display = "none";
    document.getElementById('create_user_view').style.display = "none";
}

function checkSecretary(){
    var secretary_checkbox = document.getElementById('um_issecretary');

    if(secretary_checkbox.checked === true) {
        console.log("Checkbox is checked - boolean value: ", secretary_checkbox.checked);

        getAllUsers().then(data => {
            for(item of data.managers) {
                var opt = document.createElement('option');
                opt.value = item.id;
                opt.innerHTML = item.name + " " + item.surname;
                document.getElementById('secretary_attendees').appendChild(opt);
            }
        });

        document.getElementById('secretary_attendees').style.display = "block";
    }
    if(secretary_checkbox.checked === false) {
        console.log("Checkbox is not checked - boolean value: ", secretary_checkbox.checked)
        document.getElementById('secretary_attendees').style.display = "none";
        var select = document.getElementById("secretary_attendees");
        var length = select.length;
        for (i = 0; i < length; i++) {
            select.remove(0);
        }
    }
}

function saveNewUser(){
    
    var login = document.getElementById("um_login").value;
    var password = document.getElementById("um_password").value;
    var name = document.getElementById("um_name").value;
    var surname = document.getElementById("um_surname").value;
    var date_of_birth = document.getElementById("um_date_of_birth").value;
    var is_secretary = document.getElementById("um_issecretary").checked;
    var manager_id = document.getElementById("secretary_attendees").value;
    hideUser();
    createUser(login, password, name, surname, date_of_birth, is_secretary, manager_id).then(response => {
        console.log(response);
    });
    setTimeout(function(){
        load_users();
      }, 500);
}

//NOTIFICATIONS

//Function called when signalR receives net notification
function onNotification(notification) {
    //console.log(JSON.stringify(notification, null, 2))
    /*let result = JSON.stringify(notification, null, 2);
    let item = JSON.parse(result);
    var table = document.getElementById("notifications");
    var i = table.rows.length;
    var row = table.insertRow(i);
    var title = row.insertCell(0);
    var from = row.insertCell(1);
    var until = row.insertCell(2);
    var view = row.insertCell(3);
    var dismiss = row.insertCell(4);
    title.innerHTML = notification.Meeting.Title;
    from.innerHTML = notification.Meeting.From.replace("T"," ");
    until.innerHTML = notification.Meeting.Until.replace("T"," ");
    view.innerHTML = "<button  class=\"btn\" onclick=\"showNotification('" + notification.ID + "')\"><i class=\"fa fa-pencil\"></i></button>";
    dismiss.innerHTML = "<button  class=\"btn\" onclick=\"dismiss_notification_for_id('" + notification.ID + "')\"><i class=\"fa fa-times\"></i></button>";*/
    alert(notification.Text);
    get_notifications();
    calendar.clear();
    load_manager_meetings(calendar);
}

function startSignalRNotifications() {
    //Starts signalR receiver with callback for new notification
    startSignalR(onNotification);
    //console.log("SignalR started");
}

async function get_notifications() {
    var table = document.getElementById("notifications");
    table.innerHTML = '';
    getNotifications().then(response => response.json()).then(data => {
        //console.log(data.notifications);
        var count = 0;
        for(notification of data.notifications){
            var i = table.rows.length;
            var row = table.insertRow(i);
            var title = row.insertCell(0);
            var from = row.insertCell(1);
            var until = row.insertCell(2);
            var view = row.insertCell(3);
            var dismiss = row.insertCell(4);
            title.innerHTML = notification.meeting.title;
            from.innerHTML = notification.meeting.from.replace("T"," ");
            until.innerHTML = notification.meeting.until.replace("T"," ");
            view.innerHTML = "<button  class=\"btn\" onclick=\"showNotification('" + notification.id + "')\"><i class=\"fa fa-pencil\"></i></button>";
            dismiss.innerHTML = "<button  class=\"btn\" onclick=\"dismiss_notification_for_id('" + notification.id + "')\"><i class=\"fa fa-times\"></i></button>";
            count++;
        }
        if(count>0){
            document.getElementById('notification_nav').innerHTML = "Notifications(" + count + ")";
        }
        else{
            document.getElementById('notification_nav').innerHTML = "Notifications";
        }
    });
}

async function showNotification(notification_id){
    getNotifications().then(response => response.json()).then(data => {
        for(notification of data.notifications){
            if(notification_id != notification.id)continue;

            document.getElementById('id_notif').value  = notification.id;
            document.getElementById('title_notif').innerHTML  = notification.title;
            document.getElementById('owner_notif').innerHTML  = notification.meeting.owner.name + " " + notification.meeting.owner.surname;
            document.getElementById('title_meet_notif').innerHTML  = notification.meeting.title;
            var start_datetime = changeDatetimeFormat(notification.meeting.from);
            document.getElementById('date_time_notif').innerHTML  = start_datetime.replace("T", " ") + " - " + getTimeFromDatetime(notification.meeting.until);
            document.getElementById('description_notif').innerHTML  = notification.meeting.description;
            if(notification.meeting.attendees.length !== 0){
                var logged = document.getElementById('logged_user_role').value;
                if(logged === "Ceo" || logged === "CeoSecretary"){
                    let managers = [];
                    getAllUsers().then(data => {
                    
                        for(item of data.managers) {
                            if(notification.meeting.attendees.includes(item.id)){
                                var manager = item.name + " " + item.surname;
                                //console.log("ONE MAN:" +manager);
                                managers.push(manager);
                                //console.log("ONE MAN:" +manager);
                            }
                        }
                        //console.log("Managers"+managers);
                        document.getElementById('attendees_notif').innerHTML = managers.join(', ');
                    });
                }
                else{
                    document.getElementById('attendees_notif').innerHTML = notification.meeting.attendees.length;
                }
                document.getElementById('attendees_div_notif').style.display = "block";        
            }
            else{
                document.getElementById('attendees_div_notif').style.display = "none";     
            }
            document.getElementById('notification_view').style.display = "block";
            break;
        }
    });
}

function hideNotification(){
    document.getElementById('notification_view').style.display = "none";
    document.getElementById('id_notif').value  = "";
    document.getElementById('title_notif').innerHTML  = "";
    document.getElementById('owner_notif').innerHTML  = "";
    document.getElementById('title_meet_notif').innerHTML  = "";
    document.getElementById('date_time_notif').innerHTML  = "";
    document.getElementById('description_notif').innerHTML  = "";
    document.getElementById('attendees_notif').innerHTML = "";
    document.getElementById('attendees_div_notif').style.display = "none"; 
}

async function dismiss_notification_for_id(notification_id){
    dismissNotification(notification_id).then(data => {
        console.log(data);
        get_notifications();
    });
}

async function dismiss_notification(){
    var notification_id = document.getElementById('id_notif').value;
    dismissNotification(notification_id).then(data => {
        console.log(data);
        get_notifications();
    });
}


/*function showUser(user_id){

    console.log(user_id);
    getAllUsers().then(data => {
        var found = false;
        for(user in data.managers){
            if(user.id != user_id)continue;
            document.getElementById('um_user_id').value  = user_id;
            document.getElementById('um_login').value  = user.login;
            document.getElementById('um_password').value  = "";
            document.getElementById('um_name').value  = user.name;
            document.getElementById('um_surname').value  = user.surname
            document.getElementById('um_date_of_birth').value  = changeDatetimeFormat(user.);
            var select = document.getElementById("secretary_attendees");
            var length = select.length;
            for (i = 0; i < length; i++) {
                select.remove(0);
            }
            document.getElementById('um_issecretary').checked = false; 
            document.getElementById('secretary_attendees').style.display = "none";
            document.getElementById('create_user_view').style.display = "none";
        }
    });
}*/
    /*document.getElementById('um_login').value  = "";
    document.getElementById('um_password').value  = "";
    document.getElementById('um_name').value  = "";
    document.getElementById('um_surname').value  = "";
    document.getElementById('um_name').value  = "";
    var select = document.getElementById("secretary_attendees");
    var length = select.length;
    for (i = 0; i < length; i++) {
        select.remove(0);
    }
    document.getElementById('um_issecretary').checked = false; 
    document.getElementById('secretary_attendees').style.display = "none";
    document.getElementById('create_user_view').style.display = "block";*/

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

