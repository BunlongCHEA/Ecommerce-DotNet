using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [HttpPost("room")]
    public async Task<IActionResult> CreateChatRoom([FromBody] CreateChatRoomRequest request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized("User not authenticated");
            }

            if (currentUserId != request.CustomerId && currentUserId != request.SellerId)
            {
                return Forbid("You can only create chat rooms where you are a participant");
            }

            var room = await _chatService.CreateOrGetChatRoomAsync(
                request.CustomerId,
                request.SellerId,
                request.StoreId);

            var roomResponse = new
            {
                roomId = room.Id,
                room = new
                {
                    id = room.Id,
                    customerId = room.CustomerId,
                    customerName = room.Customer?.UserName,
                    sellerId = room.SellerId,
                    sellerName = room.Seller?.UserName,
                    storeId = room.StoreId,
                    storeName = room.Store?.Name,
                    lastActivity = room.LastActivity
                }
            };

            return Ok(roomResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating chat room");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("rooms")]
    public async Task<IActionResult> GetUserChatRooms()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized("User not authenticated");
            }

            var rooms = await _chatService.GetUserChatRoomsAsync(currentUserId.Value);
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user chat rooms");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("room/{roomId}")]
    public async Task<IActionResult> GetChatRoom(int roomId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized("User not authenticated");
            }

            var hasAccess = await _chatService.UserHasAccessToRoomAsync(roomId, currentUserId.Value);
            if (!hasAccess)
            {
                return Forbid("Access denied to this chat room");
            }

            var room = await _chatService.GetChatRoomAsync(roomId);
            if (room == null)
            {
                return NotFound("Chat room not found");
            }

            var roomResponse = new
            {
                id = room.Id,
                customerId = room.CustomerId,
                customerName = room.Customer?.UserName,
                sellerId = room.SellerId,
                sellerName = room.Seller?.UserName,
                storeId = room.StoreId,
                storeName = room.Store?.Name,
                lastActivity = room.LastActivity
            };

            return Ok(roomResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting chat room {roomId}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("history/{roomId}")]
    public async Task<IActionResult> GetChatHistory(int roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized("User not authenticated");
            }

            if (pageSize > 100) pageSize = 100;

            var messages = await _chatService.GetChatHistoryAsync(roomId, currentUserId.Value, page, pageSize);
            return Ok(messages);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("Access denied to this chat room");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting chat history for room {roomId}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("message/{messageId}/read")]
    public async Task<IActionResult> MarkMessageAsRead(int messageId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized("User not authenticated");
            }

            await _chatService.MarkMessageAsReadAsync(messageId, currentUserId.Value);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marking message {messageId} as read");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadMessagesCount()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized("User not authenticated");
            }

            var count = await _chatService.GetUnreadMessagesCountAsync(currentUserId.Value);
            return Ok(new { unreadCount = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread messages count");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("room/{roomId}/unread-count")]
    public async Task<IActionResult> GetUnreadMessagesCountForRoom(int roomId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized("User not authenticated");
            }

            var hasAccess = await _chatService.UserHasAccessToRoomAsync(roomId, currentUserId.Value);
            if (!hasAccess)
            {
                return Forbid("Access denied to this chat room");
            }

            var count = await _chatService.GetUnreadMessagesCountForRoomAsync(roomId, currentUserId.Value);
            return Ok(new { unreadCount = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting unread messages count for room {roomId}");
            return StatusCode(500, "Internal server error");
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}

// Simple request model for creating chat room
public class CreateChatRoomRequest
{
    public int CustomerId { get; set; }
    public int SellerId { get; set; }
    public int StoreId { get; set; }
}