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

async function removeMeeting(meetingId) {
    return fetch("/Meeting/DeleteMeeting?" + new URLSearchParams({
        meetingId: meetingId
    }),
        {
            method: "DELETE"
        }
    );
}

async function createMeeting(title, description, from, until, attendees) {
    let body = {
        "title": title,
        "description": description,
        "from": from,
        "until": until,
        "attendees": attendees
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