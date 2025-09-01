using MongoDB.Driver;

public interface IMongoDbService
{
    IMongoDatabase Database { get; }
    IMongoCollection<ChatImage> ChatImages { get; }
    Task<string> SaveImageAsync(ChatImage image);
    Task<ChatImage?> GetImageAsync(string imageId);
    Task<bool> DeleteImageAsync(string imageId);
    Task<List<ChatImage>> GetImagesByChatRoomAsync(int chatRoomId);
}