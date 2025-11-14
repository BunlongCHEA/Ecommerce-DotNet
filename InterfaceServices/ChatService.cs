using ECommerceAPI.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChatService> _logger;

    public ChatService(ApplicationDbContext context, ILogger<ChatService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ChatRoom> CreateOrGetChatRoomAsync(int customerId, int sellerId, int storeId)
    {
        try
        {
            var existingRoom = await _context.ChatRooms
                .Include(r => r.Customer)
                .Include(r => r.Seller)
                .Include(r => r.Store)
                .FirstOrDefaultAsync(r => r.CustomerId == customerId &&
                                          r.SellerId == sellerId &&
                                          r.StoreId == storeId);

            if (existingRoom != null)
            {
                existingRoom.LastActivity = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
                return existingRoom;
            }

            var newRoom = new ChatRoom
            {
                CustomerId = customerId,
                SellerId = sellerId,
                StoreId = storeId,
                LastActivity = DateTimeOffset.UtcNow
            };

            _context.ChatRooms.Add(newRoom);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            return await _context.ChatRooms
                .Include(r => r.Customer)
                .Include(r => r.Seller)
                .Include(r => r.Store)
                .FirstOrDefaultAsync(r => r.Id == newRoom.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating/getting chat room");
            throw;
        }
    }

    public async Task<List<object>> GetChatHistoryAsync(int roomId, int userId, int page = 1, int pageSize = 50)
    {
        try
        {
            if (!await UserHasAccessToRoomAsync(roomId, userId))
            {
                throw new UnauthorizedAccessException("User does not have access to this chat room");
            }

            var messages = await _context.ChatMessages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .Where(m => m.ChatRoomId == roomId)
                    .OrderBy(m => m.Timestamp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new
                    {
                        id = m.Id,
                        senderId = m.SenderId,
                        senderName = m.Sender != null ? m.Sender.UserName : "Unknown",
                        receiverId = m.ReceiverId,
                        receiverName = m.Receiver != null ? m.Receiver.UserName : "Unknown",
                        message = m.Message,
                        imageId = m.ImageId,
                        linkUrl = m.LinkUrl,
                        timestamp = m.Timestamp,
                        isRead = m.IsRead,
                        chatRoomId = m.ChatRoomId
                    })
                    .ToListAsync<object>();

            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting chat history for room {roomId}");
            throw;
        }
    }

    public async Task<ChatMessage?> SendMessageAsync(int roomId, int senderId, string message, string? imageId = null)
    {
        try
        {
            if (!await UserHasAccessToRoomAsync(roomId, senderId))
            {
                throw new UnauthorizedAccessException("User does not have access to this chat room");
            }

            var chatRoom = await _context.ChatRooms.FindAsync(roomId);
            if (chatRoom == null)
            {
                throw new ArgumentException("Chat room not found");
            }

            var receiverId = chatRoom.CustomerId == senderId ? chatRoom.SellerId : chatRoom.CustomerId;

            // Extract URLs from message
            string? extractedUrl = null;
            if (!string.IsNullOrEmpty(message))
            {
                extractedUrl = ExtractUrlFromMessage(message);
            }

            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                ImageId = imageId,
                LinkUrl = extractedUrl,
                Timestamp = DateTimeOffset.UtcNow,
                IsRead = false,
                ConnectionType = "customer-seller",
                ChatRoomId = roomId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            chatRoom.LastActivity = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            // Load navigation properties
            return await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstAsync(m => m.Id == chatMessage.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending message in room {roomId}");
            throw;
        }
    }

    public async Task MarkMessageAsReadAsync(int messageId, int userId)
    {
        try
        {
            var message = await _context.ChatMessages
                .Where(m => m.Id == messageId && m.ReceiverId == userId)
                .FirstOrDefaultAsync();

            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marking message {messageId} as read");
            throw;
        }
    }

    public async Task<List<object>> GetUserChatRoomsAsync(int userId)
    {
        try
        {
            var rooms = await _context.ChatRooms
                    .Include(r => r.Customer)
                    .Include(r => r.Seller)
                    .Include(r => r.Store)
                    .Where(r => r.CustomerId == userId || r.SellerId == userId)
                    .OrderByDescending(r => r.LastActivity)
                    .Select(r => new
                    {
                        id = r.Id,
                        customerId = r.CustomerId,
                        customerName = r.Customer != null ? r.Customer.UserName : "Unknown",
                        sellerId = r.SellerId,
                        sellerName = r.Seller != null ? r.Seller.UserName : "Unknown",
                        storeId = r.StoreId,
                        storeName = r.Store != null ? r.Store.Name : "Unknown",
                        lastActivity = r.LastActivity,
                        lastMessage = r.Messages!
                            .OrderByDescending(m => m.Timestamp)
                            .Select(m => m.Message)
                            .FirstOrDefault(),
                        unreadCount = r.Messages!
                            .Count(m => m.ReceiverId == userId && !m.IsRead)
                    })
                    .ToListAsync<object>();

            return rooms;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting chat rooms for user {userId}");
            throw;
        }
    }

    public async Task<bool> UserHasAccessToRoomAsync(int roomId, int userId)
    {
        try
        {
            return await _context.ChatRooms
                .AnyAsync(r => r.Id == roomId && (r.CustomerId == userId || r.SellerId == userId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking user access to room {roomId}");
            return false;
        }
    }

    public async Task<ChatRoom?> GetChatRoomAsync(int roomId)
    {
        try
        {
            return await _context.ChatRooms
                .Include(r => r.Customer)
                .Include(r => r.Seller)
                .Include(r => r.Store)
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting chat room {roomId}");
            throw;
        }
    }

    public async Task<int> GetUnreadMessagesCountAsync(int userId)
    {
        try
        {
            return await _context.ChatMessages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting unread messages count for user {userId}");
            return 0;
        }
    }

    public async Task<int> GetUnreadMessagesCountForRoomAsync(int roomId, int userId)
    {
        try
        {
            return await _context.ChatMessages
                .CountAsync(m => m.ChatRoomId == roomId && m.ReceiverId == userId && !m.IsRead);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting unread messages count for room {roomId}");
            return 0;
        }
    }

    public async Task<List<ChatRoom?>> GetAdminSupportRoomsAsync()
    {
        try
        {
            // Get all chat rooms where seller is an admin
            // Assuming StoreId = -1 or null for support rooms (based on your CreateSupportRoom logic)
            var adminRooms = await _context.ChatRooms
                .Include(r => r.Customer)
                .Include(r => r.Seller)
                .Include(r => r.Store)
                .Include(r => r.Messages)
                .Where(r => r.Seller.Role == "Admin" && (r.StoreId == -1 || r.StoreId == null))
                .OrderByDescending(r => r.LastActivity)
                .Select(r => new ChatRoom
                {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    // CustomerName = (r.Customer.FirstName ?? "") + " " + (r.Customer.LastName ?? ""),
                    SellerId = r.SellerId,
                    // SellerName = (r.Seller.FirstName ?? "") + " " + (r.Seller.LastName ?? ""),
                    StoreId = r.StoreId,
                    // StoreName = r.Store != null ? r.Store.Name : "Support",
                    LastActivity = r.LastActivity
                    // LastMessage = r.Messages
                    //     .OrderByDescending(m => m.Timestamp)
                    //     .Select(m => m.Message)
                    //     .FirstOrDefault() ?? "",
                    // UnreadCount = r.Messages
                    //     .Count(m => m.ReceiverId == r.SellerId && !m.IsRead)
                })
                .ToListAsync();

            return adminRooms;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting admin support rooms");
            throw;
        }
    }

    public async Task<List<int>> GetAdminUserIdsAsync()
    {
        try
        {
            var adminIds = await _context.Users
                .Where(u => u.Role == "Admin")
                .Select(u => u.Id)
                .ToListAsync();

            return adminIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting admin user IDs");
            throw;
        }
    }

    private string? ExtractUrlFromMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return null;

        var urlPattern = @"https?://[^\s]+";
        var match = System.Text.RegularExpressions.Regex.Match(message, urlPattern);
        return match.Success ? match.Value : null;
    }
}