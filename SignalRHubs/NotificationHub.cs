using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WAP_PIS.SignalRHubs;

[Authorize]
public class NotificationHub: Hub
{
    private static readonly ConcurrentDictionary<string, string> connectedUsers = new ConcurrentDictionary<string, string>();
    public static ReadOnlyDictionary<string, string> ConnectedUsers { get; } = new ReadOnlyDictionary<string, string>(connectedUsers);
    
    public override Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var connectionString = Context.ConnectionId;
        connectedUsers[userId] = connectionString;
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        connectedUsers.TryRemove(userId, out _);
        return base.OnDisconnectedAsync(exception);
    }

}
