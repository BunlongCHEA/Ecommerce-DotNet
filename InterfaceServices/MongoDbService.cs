using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MongoDbService : IMongoDbService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<ChatImage> _chatImages;

    public MongoDbService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.MongoDB);
        _database = client.GetDatabase(settings.Value.DatabaseName);
        _chatImages = _database.GetCollection<ChatImage>(settings.Value.Collections.ChatImages);
    }

    public IMongoDatabase Database => _database;
    public IMongoCollection<ChatImage> ChatImages => _chatImages;

    public async Task<string> SaveImageAsync(ChatImage image)
    {
        await _chatImages.InsertOneAsync(image);
        return image.Id;
    }

    public async Task<ChatImage?> GetImageAsync(string imageId)
    {
        return await _chatImages.Find(img => img.Id == imageId).FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteImageAsync(string imageId)
    {
        var result = await _chatImages.DeleteOneAsync(img => img.Id == imageId);
        return result.DeletedCount > 0;
    }

    public async Task<List<ChatImage>> GetImagesByChatRoomAsync(int chatRoomId)
    {
        return await _chatImages.Find(img => img.ChatRoomId == chatRoomId)
            .SortByDescending(img => img.UploadedAt)
            .ToListAsync();
    }
}