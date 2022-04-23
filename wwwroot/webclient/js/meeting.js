async function getMeetings() {
    return fetch("/Meeting/GetMeetings");
}

async function updateMeeting(meetingId, newTitle = null, newDescription = null, newFrom = null, newUntil = null) {
    let body = {
        "title": newTitle,
        "description": newDescription,
        "from": newFrom,
        "until": newUntil
    };

    return fetch("/Meeting/UpdateMeeting?" + new URLSearchParams({
        meetingId: meetingId
    }),
        {
            method: 'PATCH',
            body: JSON.stringify(body),
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

async function updateGroupMeeting(meetingId, newTitle = null, newDescription = null, newFrom = null, newUntil = null, attendees = null) {
    let body = {
        "title": newTitle,
        "description": newDescription,
        "from": newFrom,
        "until": newUntil,
        "attendees": attendees
    };

    return fetch("/Meeting/UpdateGroupMeeting?" + new URLSearchParams({
        meetingId: meetingId
    }),
        {
            method: 'PATCH',
            body: JSON.stringify(body),
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

async function removeMeeting(meetingId) {
    return fetch("/Meeting/DeleteMeeting?" + new URLSearchParams({
        meetingId: meetingId
    }),
        {
            method: "DELETE"
        }
    );
}

async function getMeetingsForUser(userId) {
    return fetch("/Meeting/GetMeetingsForUser?" + new URLSearchParams({
        userId: userId
    }),
        {
            method: "GET"
        }
    );
}

async function getAllMeetings() {
    return fetch("/Meeting/GetAllMeetings",
        {
            method: "GET"
        }
    );
}

async function createGroupMeeting(title, description, from, until, attendees) {
    let body = {
        "title": title,
        "description": description,
        "from": from,
        "until": until,
        "attendees": attendees
    };
    return await fetch("/Meeting/CreateGroupMeeting",
        {
            method: 'POST',
            body: JSON.stringify(body),
            headers: {
                'Content-Type': 'application/json'
            }
        }
    );
}

async function createMeeting(title, description, from, until) {
    let body = {
        "title": title,
        "description": description,
        "from": from,
        "until": until,
    };
    return await fetch("/Meeting/CreateMeeting",
        {
            method: 'POST',
            body: JSON.stringify(body),
            headers: {
                'Content-Type': 'application/json'
            }
        }
    );
}
