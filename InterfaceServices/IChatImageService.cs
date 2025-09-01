public interface IChatImageService
{
    Task<string> UploadImageAsync(IFormFile file, int uploadedBy, int chatRoomId, string? description = null);
    Task<ChatImage?> GetImageAsync(string imageId);
    Task<bool> DeleteImageAsync(string imageId, int userId);
    Task<List<ChatImage>> GetChatRoomImagesAsync(int chatRoomId, int userId);
}