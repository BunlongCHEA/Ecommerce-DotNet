using ECommerceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Requires authentication
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IChatService _chatService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        ApplicationDbContext context, 
        IChatService chatService,
        ILogger<AdminController> logger)
    {
        _context = context;
        _chatService = chatService;
        _logger = logger;
    }

    // Get an available admin user for customer support
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableAdmin()
    {
        try
        {
            // Get the first available admin user
            var admin = await _context.Users
                .Where(u => u.Role == "Admin")
                .Select(u => new { 
                    id = u.Id, 
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email 
                })
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                return NotFound(new { message = "No admin users available" });
            }

            return Ok(admin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding admin user");
            return StatusCode(500, new { message = "Error finding admin user" });
        }
    }

    // Create a support chat room between current user and admin
    [HttpPost("support-room")]
    public async Task<IActionResult> CreateSupportRoom()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            // Don't allow admins to create support rooms with themselves
            var currentUser = await _context.Users.FindAsync(currentUserId);
            if (currentUser?.Role == "Admin")
            {
                return BadRequest(new { message = "Admin users cannot create support rooms" });
            }

            // Get an available admin
            var admin = await _context.Users
                .Where(u => u.Role == "Admin")
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                return NotFound(new { message = "No admin users available" });
            }

            // Create or get existing chat room
            // Using StoreId = 1 as default for support chats (you can modify this logic)
            var room = await _chatService.CreateOrGetChatRoomAsync(
                currentUserId.Value,
                admin.Id,
                currentUser?.StoreId ?? 1);

            var roomResponse = new
            {
                roomId = room.Id,
                adminName = admin.FirstName + " " + admin.LastName,
                room = new
                {
                    id = room.Id,
                    customerId = room.CustomerId,
                    customerName = room.Customer?.FirstName + " " + room.Customer?.LastName,
                    sellerId = room.SellerId,
                    sellerName = room.Seller?.FirstName + " " + room.Seller?.LastName,
                    storeId = room.StoreId,
                    storeName = room.Store?.Name ?? "Support",
                    lastActivity = room.LastActivity
                }
            };

            return Ok(roomResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating support room");
            return StatusCode(500, new { message = "Error creating support room" });
        }
    }

    // Get unread message count for current user
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var count = await _chatService.GetUnreadMessagesCountAsync(currentUserId.Value);
            return Ok(new { unreadCount = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count");
            return StatusCode(500, new { message = "Error retrieving unread count" });
        }
    }

    // Get all support chat rooms (Admin only)
    [HttpGet("support-rooms")]
    [Authorize(Policy = "Admin")] // Only admins can access this
    public async Task<IActionResult> GetSupportRooms()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            // Get all chat rooms where current admin is the seller (support agent)
            var rooms = await _chatService.GetUserChatRoomsAsync(currentUserId.Value);
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting support rooms");
            return StatusCode(500, new { message = "Error getting support rooms" });
        }
    }

    // Get admin dashboard statistics
    [HttpGet("dashboard-stats")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var stats = new
            {
                totalUsers = await _context.Users.CountAsync(u => u.Role == "User"),
                totalAdmins = await _context.Users.CountAsync(u => u.Role == "Admin"),
                activeChatRooms = await _context.ChatRooms
                    .CountAsync(r => r.SellerId == currentUserId && 
                                r.LastActivity >= DateTimeOffset.UtcNow.AddDays(-7)),
                unreadMessages = await _context.ChatMessages
                    .CountAsync(m => m.ReceiverId == currentUserId && !m.IsRead),
                todayMessages = await _context.ChatMessages
                    .CountAsync(m => m.Timestamp >= DateTimeOffset.UtcNow.Date)
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return StatusCode(500, new { message = "Error getting dashboard stats" });
        }
    }

    // Helper method to get current user ID from JWT claims
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}