public interface ICloudStorageService
{
    Task<string> UploadImageAsync(IFormFile file, string fileName);
    Task<bool> DeleteImageAsync(string fileName);
    // string GetImageUrl(string fileName);
    Task<bool> TestConnectionAsync();
}