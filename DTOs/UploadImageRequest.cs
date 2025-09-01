public class UploadImageRequest
{
    public IFormFile File { get; set; } = null!;
    public int ChatRoomId { get; set; }
    public string? Description { get; set; }
}