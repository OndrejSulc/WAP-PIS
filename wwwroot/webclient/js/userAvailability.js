async function isUserAvailable(userId, from, to, ignoreMeeting = null) {
    let urlParams = new URLSearchParams({
        user: userId,
        from: from,
        to: to,
    });
    if(ignoreMeeting !== null) {
        urlParams.append("ignoreMeeting", ignoreMeeting);
    }
    
    return fetch("/UserAvailability/isAvailable?" + urlParams);
}

async function isCeoAvailable(from, to) {
    return fetch("/UserAvailability/isCEOAvailable?" + new URLSearchParams({
        from: from,
        to: to
    }))
}
