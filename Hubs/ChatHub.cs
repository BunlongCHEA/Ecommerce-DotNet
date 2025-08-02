using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    public async Task JoinRoom(string roomId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            await Clients.Caller.SendAsync("Error", "Unauthorized access");
            return;
        }

        var hasAccess = await _chatService.UserHasAccessToRoomAsync(int.Parse(roomId), userId.Value);
        if (!hasAccess)
        {
            await Clients.Caller.SendAsync("Error", "Access denied to this chat room");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"ChatRoom_{roomId}");
        _logger.LogInformation($"User {userId} joined room {roomId}");
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ChatRoom_{roomId}");
        var userId = GetCurrentUserId();
        _logger.LogInformation($"User {userId} left room {roomId}");
    }

    public async Task SendMessage(string roomId, string message)
    {
        var senderId = GetCurrentUserId();
        if (senderId == null)
        {
            await Clients.Caller.SendAsync("Error", "Unauthorized access");
            return;
        }

        try
        {
            var chatMessage = await _chatService.SendMessageAsync(int.Parse(roomId), senderId.Value, message);
            
            if (chatMessage != null)
            {
                var messageResponse = new
                {
                    id = chatMessage.Id,
                    senderId = chatMessage.SenderId,
                    senderName = chatMessage.Sender?.UserName ?? "Unknown",
                    receiverId = chatMessage.ReceiverId,
                    message = chatMessage.Message,
                    timestamp = chatMessage.Timestamp,
                    isRead = chatMessage.IsRead,
                    chatRoomId = chatMessage.ChatRoomId
                };

                await Clients.Group($"ChatRoom_{roomId}").SendAsync("ReceiveMessage", messageResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending message in room {roomId}");
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    public async Task MarkAsRead(string messageId)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return;

        try
        {
            await _chatService.MarkMessageAsReadAsync(int.Parse(messageId), userId.Value);
            await Clients.All.SendAsync("MessageRead", messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marking message {messageId} as read");
        }
    }

    public async Task UserTyping(string roomId)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return;

        await Clients.OthersInGroup($"ChatRoom_{roomId}").SendAsync("UserTyping", userId);
    }

    public async Task UserStoppedTyping(string roomId)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return;

        await Clients.OthersInGroup($"ChatRoom_{roomId}").SendAsync("UserStoppedTyping", userId);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        if (userId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            _logger.LogInformation($"User {userId} connected to SignalR");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetCurrentUserId();
        if (userId != null)
        {
            _logger.LogInformation($"User {userId} disconnected from SignalR");
        }
        await base.OnDisconnectedAsync(exception);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}