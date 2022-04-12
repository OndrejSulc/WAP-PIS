async function getNotifications() {
    return fetch("/Notification/GetNotifications")
}

async function dismissNotification(notificationId) {
    return fetch("/Notification/DismissNotification?" + new URLSearchParams({
        notificationId: notificationId
    }),
        {
            method: "POST"
        }
    );
}
