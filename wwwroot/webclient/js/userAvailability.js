async function isUserAvailable(userId, from, to) {
    return fetch("/UserAvailability/isAvailable?" + new URLSearchParams({
        user: userId,
        from: from,
        to: to
    }))
}

async function isCeoAvailable(from, to) {
    return fetch("/UserAvailability/isCEOAvailable?" + new URLSearchParams({
        from: from,
        to: to
    }))
}
