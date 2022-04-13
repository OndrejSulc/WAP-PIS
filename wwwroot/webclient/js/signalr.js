async function startSignalR(onNotification) {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();
    connection.on("NotificationAdded", message => {
        onNotification(JSON.parse(message))
    })
    try {
        await connection.start();
        console.log("SignalR Connected");
    } catch (err) {
        console.log(err);
    }
}
