public class ChatImageService : IChatImageService
{
    private readonly IMongoDbService _mongoDb;
    private readonly IChatService _chatService;
    private readonly ILogger<ChatImageService> _logger;

    // Allowed image types
    private readonly string[] _allowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public ChatImageService(IMongoDbService mongoDb, IChatService chatService, ILogger<ChatImageService> logger)
    {
        _mongoDb = mongoDb;
        _chatService = chatService;
        _logger = logger;
    }

    public async Task<string> UploadImageAsync(IFormFile file, int uploadedBy, int chatRoomId, string? description = null)
    {
        try
        {
            // Validate file
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            if (file.Length > MaxFileSize)
                throw new ArgumentException("File size exceeds limit");

            if (!_allowedImageTypes.Contains(file.ContentType.ToLower()))
                throw new ArgumentException("Invalid file type");

            // Check if user has access to chat room
            if (!await _chatService.UserHasAccessToRoomAsync(chatRoomId, uploadedBy))
                throw new UnauthorizedAccessException("User does not have access to this chat room");

            // Read file data
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var imageData = memoryStream.ToArray();

            // Create image document
            var chatImage = new ChatImage
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                ImageData = imageData,
                FileSize = file.Length,
                UploadedBy = uploadedBy,
                ChatRoomId = chatRoomId,
                Description = description,
                UploadedAt = DateTimeOffset.UtcNow
            };

            // Optional: Get image dimensions (requires System.Drawing or ImageSharp)
            // You can implement this later if needed

            return await _mongoDb.SaveImageAsync(chatImage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to chat room {ChatRoomId}", chatRoomId);
            throw;
        }
    }

    public async Task<ChatImage?> GetImageAsync(string imageId)
    {
        try
        {
            return await _mongoDb.GetImageAsync(imageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting image {ImageId}", imageId);
            throw;
        }
    }

    public async Task<bool> DeleteImageAsync(string imageId, int userId)
    {
        try
        {
            var image = await _mongoDb.GetImageAsync(imageId);
            if (image == null)
                return false;

            // Check if user can delete (owner or has access to chat room)
            if (image.UploadedBy != userId && 
                !await _chatService.UserHasAccessToRoomAsync(image.ChatRoomId, userId))
            {
                throw new UnauthorizedAccessException("User does not have permission to delete this image");
            }

            return await _mongoDb.DeleteImageAsync(imageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {ImageId}", imageId);
            throw;
        }
    }

    public async Task<List<ChatImage>> GetChatRoomImagesAsync(int chatRoomId, int userId)
    {
        try
        {
            if (!await _chatService.UserHasAccessToRoomAsync(chatRoomId, userId))
                throw new UnauthorizedAccessException("User does not have access to this chat room");

            return await _mongoDb.GetImagesByChatRoomAsync(chatRoomId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting images for chat room {ChatRoomId}", chatRoomId);
            throw;
        }
    }
}