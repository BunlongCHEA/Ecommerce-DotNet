public interface ICloudStorageService
{
    Task<string> UploadImageAsync(IFormFile file, string fileName);
    Task<bool> DeleteImageAsync(string fileName);
    // string GetImageUrl(string fileName);
    Task<bool> TestConnectionAsync();
    Task<Dictionary<string, string>> UploadBatchImagesAsync(Dictionary<string, IFormFile> imageFiles);
}