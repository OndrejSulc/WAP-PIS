async function isUserAvailable(userId, from, to) {
    return fetch("/UserAvailability/isAvailable?" + new URLSearchParams({
        user: userId,
        from: from.toISOString(),
        to: to.toISOString()
    }))
}

async function isCeoAvailable(from, to) {
    return fetch("/UserAvailability/isCEOAvailable?" + new URLSearchParams({
        from: from.toISOString(),
        to: to.toISOString()
    }))
}
