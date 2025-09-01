using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

public class ChatImage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    public byte[] ImageData { get; set; } = Array.Empty<byte>();

    public long FileSize { get; set; }

    public int UploadedBy { get; set; } // User ID who uploaded the image

    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;

    public int ChatRoomId { get; set; }

    public string? Description { get; set; }

    // Image dimensions
    public int? Width { get; set; }
    public int? Height { get; set; }
}