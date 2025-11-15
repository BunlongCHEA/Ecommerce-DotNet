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
                    imageId = chatMessage.ImageId,
                    linkUrl = chatMessage.LinkUrl,
                    timestamp = chatMessage.Timestamp,
                    isRead = chatMessage.IsRead,
                    chatRoomId = chatMessage.ChatRoomId,
                    hasImage = !string.IsNullOrEmpty(chatMessage.ImageId)
                };

                // Broadcast message to room
                await Clients.Group($"ChatRoom_{roomId}").SendAsync("ReceiveMessage", messageResponse);

                // Update unread count for receiver (if not currently in room)
                var receiverUnreadCount = await _chatService.GetUnreadMessagesCountAsync(chatMessage.ReceiverId);
                await Clients.Group($"User_{chatMessage.ReceiverId}").SendAsync("UnreadCountUpdate", receiverUnreadCount);

                // Update admin rooms if receiver is not admin
                var adminUsers = await _chatService.GetAdminUserIdsAsync(); // You need to implement this
                foreach (var adminId in adminUsers)
                {
                    var rooms = await _chatService.GetAdminSupportRoomsAsync();
                    await Clients.Group($"User_{adminId}").SendAsync("AdminRoomsUpdate", rooms);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending message in room {roomId}");
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    public async Task SendMessageWithImage(string roomId, string message, string imageId)
    {
        var senderId = GetCurrentUserId();
        if (senderId == null)
        {
            await Clients.Caller.SendAsync("Error", "Unauthorized access");
            return;
        }

        try
        {
            // Call SendMessageAsync with imageId
            var chatMessage = await _chatService.SendMessageAsync(int.Parse(roomId), senderId.Value, message, imageId);
            
            if (chatMessage != null)
            {
                var messageResponse = new
                {
                    id = chatMessage.Id,
                    senderId = chatMessage.SenderId,
                    senderName = chatMessage.Sender?.UserName ?? "Unknown",
                    receiverId = chatMessage.ReceiverId,
                    message = chatMessage.Message,
                    imageId = chatMessage.ImageId,
                    linkUrl = chatMessage.LinkUrl,
                    timestamp = chatMessage.Timestamp,
                    isRead = chatMessage.IsRead,
                    chatRoomId = chatMessage.ChatRoomId,
                    hasImage = !string.IsNullOrEmpty(chatMessage.ImageId)
                };

                await Clients.Group($"ChatRoom_{roomId}").SendAsync("ReceiveMessage", messageResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending message with image in room {roomId}");
            await Clients.Caller.SendAsync("Error", "Failed to send message with image");
        }
    }

    public async Task MarkAsRead(string messageId)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return;

        try
        {
            await _chatService.MarkMessageAsReadAsync(int.Parse(messageId), userId.Value);

            // Notify all clients that message was read
            await Clients.All.SendAsync("MessageRead", messageId);

            // Update unread count for the user who read the message
            var unreadCount = await _chatService.GetUnreadMessagesCountAsync(userId.Value);
            await Clients.Group($"User_{userId}").SendAsync("UnreadCountUpdate", unreadCount);

            // Update admin rooms
            var adminUsers = await _chatService.GetAdminUserIdsAsync();
            foreach (var adminId in adminUsers)
            {
                var rooms = await _chatService.GetAdminSupportRoomsAsync();
                await Clients.Group($"User_{adminId}").SendAsync("AdminRoomsUpdate", rooms);
            }
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

    // Add these methods to your ChatHub class
    public async Task SubscribeToUnreadCount()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            await Clients.Caller.SendAsync("Error", "Unauthorized access");
            return;
        }

        try
        {
            // Get initial unread count
            var unreadCount = await _chatService.GetUnreadMessagesCountAsync(userId.Value);
            
            // Send initial count to caller
            await Clients.Caller.SendAsync("UnreadCountUpdate", unreadCount);
            
            _logger.LogInformation($"User {userId} subscribed to unread count updates");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error subscribing to unread count for user {userId}");
            await Clients.Caller.SendAsync("Error", "Failed to subscribe to unread count");
        }
    }

    public async Task GetAdminRoomsUpdate()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            await Clients.Caller.SendAsync("Error", "Unauthorized access");
            return;
        }

        try
        {
            // Check if user is admin
            var isAdmin = Context.User?.IsInRole("Admin") ?? false;
            if (!isAdmin)
            {
                await Clients.Caller.SendAsync("Error", "Access denied - Admin only");
                return;
            }

            // Get admin support rooms
            // var rooms = await _chatService.GetAdminSupportRoomsAsync();
            var rooms = await _chatService.GetUserChatRoomsAsync(userId.Value);
            
            // Send rooms to caller
            await Clients.Caller.SendAsync("AdminRoomsUpdate", rooms);
            
            _logger.LogInformation($"Admin {userId} received rooms update");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting admin rooms for user {userId}");
            await Clients.Caller.SendAsync("Error", "Failed to get admin rooms");
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}