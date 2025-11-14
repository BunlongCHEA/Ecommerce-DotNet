public interface IChatService
{
    Task<ChatRoom> CreateOrGetChatRoomAsync(int customerId, int sellerId, int storeId);
    Task<List<object>> GetChatHistoryAsync(int roomId, int userId, int page = 1, int pageSize = 50);
    Task<ChatMessage?> SendMessageAsync(int roomId, int senderId, string? message, string? imageId = null);
    Task MarkMessageAsReadAsync(int messageId, int userId);
    Task<List<object>> GetUserChatRoomsAsync(int userId);
    Task<bool> UserHasAccessToRoomAsync(int roomId, int userId);
    Task<ChatRoom?> GetChatRoomAsync(int roomId);
    Task<int> GetUnreadMessagesCountAsync(int userId);
    Task<int> GetUnreadMessagesCountForRoomAsync(int roomId, int userId);
    Task<List<ChatRoom?>> GetAdminSupportRoomsAsync();
    Task<List<int>> GetAdminUserIdsAsync();
}