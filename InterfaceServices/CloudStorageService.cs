using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;

public class CloudStorageService : ICloudStorageService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;
    private readonly string _baseUrl;
    private readonly ILogger<CloudStorageService> _logger;
    private readonly GoogleCredential _credential; // Store credential for URL signing

    public CloudStorageService(IConfiguration configuration, ILogger<CloudStorageService> logger)
    {
        // _storageClient = StorageClient.Create();
        _bucketName = configuration["CloudStorage:BucketName"] ?? throw new ArgumentNullException("CloudStorage:BucketName configuration is missing.");
        _baseUrl = configuration["CloudStorage:BaseUrl"] ?? throw new ArgumentNullException("CloudStorage:BaseUrl configuration is missing."); // For flexibility to change cloud providers
        _logger = logger;

        try
        {
            // _storageClient = CreateStorageClient(configuration);
            (_storageClient, _credential) = CreateFromBase64Json(configuration);
            _logger.LogInformation("Google Cloud Storage client initialized successfully using {Method}",
                configuration["CloudStorage:AuthenticationMethod"]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Google Cloud Storage client");
            throw;
        }
    }
    
    // Create StorageClient based on authentication method with Base64Json
    private (StorageClient storageClient, GoogleCredential credential) CreateFromBase64Json(IConfiguration configuration)
    {
        var base64Credential = configuration["CloudStorage:CredentialBase64"];
        if (string.IsNullOrEmpty(base64Credential))
        {
            throw new InvalidOperationException("CredentialBase64 is required when using Base64Json authentication method");
        }

        try
        {
            var jsonBytes = Convert.FromBase64String(base64Credential);
            var jsonContent = Encoding.UTF8.GetString(jsonBytes);
            var credential = GoogleCredential.FromJson(jsonContent)
                    .CreateScoped("https://www.googleapis.com/auth/cloud-platform");

            var storageClient = StorageClient.Create(credential);
            _logger.LogInformation("Successfully created Google Cloud Storage client from base64 credentials");

            return (storageClient, credential);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decode base64 credentials");
            throw new InvalidOperationException("Invalid base64 credentials format", ex);
        }
    }

    public async Task<string> UploadImageAsync(IFormFile file, string fileName)
    {
        _logger.LogInformation("Starting upload for file: {FileName}, Size: {Size} bytes", fileName, file.Length);
    
        try
        {
            // Validate file
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty");
            }

            _logger.LogInformation("File validation passed for: {FileName}", fileName);

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException($"Invalid file type: {file.ContentType}. Allowed types: {string.Join(", ", allowedTypes)}");
            }

            // Validate file size (e.g., max 5MB)
            const int maxSizeBytes = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxSizeBytes)
            {
                throw new ArgumentException($"File size exceeds maximum allowed size of {maxSizeBytes / (1024 * 1024)}MB");
            }

            _logger.LogInformation("Starting Google Cloud upload for: {FileName}", fileName);

            using var stream = file.OpenReadStream();
            var objectName = $"products/{fileName}";

            _logger.LogInformation("Uploading to bucket: {BucketName}, objectName: {ObjectName}", _bucketName, objectName);

            // Upload the file
            var uploadedObject = await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: objectName,
                contentType: file.ContentType,
                source: stream,
                options: null,
                cancellationToken: CancellationToken.None);

            _logger.LogInformation("Upload completed for: {FileName}", fileName);

            // Set cache control after upload
            try
            {
                uploadedObject.CacheControl = "public, max-age=86400"; // Cache for 1 day
                await _storageClient.UpdateObjectAsync(uploadedObject);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to set cache control for uploaded object: {ObjectName}", objectName);
                // Continue execution as this is not critical
            }

            _logger.LogInformation("Image uploaded successfully: {FileName} -> {ObjectName}", fileName, objectName);

            // Option 1: Make object public and return public URL
            var isPublic = await MakeObjectPublicAsync(objectName);
            if (isPublic)
            {
                var publicUrl = GetPublicImageUrl(fileName);
                _logger.LogInformation("Object made public, returning public URL: {PublicUrl}", publicUrl);
                return publicUrl;
            }

            // Option 2: Generate signed URL (fallback if making public fails)
            var SignedUrl = await GenerateSignedUrlAsync(objectName, TimeSpan.FromDays(365));
            // var imageUrl = GetImageUrl(objectName);
            _logger.LogInformation("Generated signed URL: {SignedUrl}", SignedUrl);

            return SignedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload image: {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> DeleteImageAsync(string fileName)
    {
        try
        {
            var objectName = fileName.StartsWith("products/") ? fileName : $"products/{fileName}";
            await _storageClient.DeleteObjectAsync(_bucketName, objectName);
            _logger.LogInformation("Image deleted successfully: {FileName}", fileName);
            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Image not found for deletion: {FileName}", fileName);
            return true; // Consider it successful if already deleted
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image: {FileName}", fileName);
            return false;
        }
    }

    // Make object publicly accessible
    public async Task<bool> MakeObjectPublicAsync(string objectName)
    {
        try
        {
            // Get the current object
            var storageObject = await _storageClient.GetObjectAsync(_bucketName, objectName);
            
            // Create new ACL with public read access
            var newAcl = new List<ObjectAccessControl>
            {
                new ObjectAccessControl
                {
                    Entity = "allUsers",
                    Role = "READER"
                }
            };

            // Update the object's ACL
            storageObject.Acl = newAcl;
            await _storageClient.UpdateObjectAsync(storageObject);
            
            _logger.LogInformation("Successfully made object public: {ObjectName}", objectName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to make object public: {ObjectName}. Error: {Error}", objectName, ex.Message);
            return false;
        }
    }

    // Generate a signed URL for the specified object, return Signed URL for accessing the object
    public async Task<string> GenerateSignedUrlAsync(string objectName, TimeSpan? duration = null)
    {
        try
        {
            var urlDuration = duration ?? TimeSpan.FromDays(365); // Default to 1 year
            var urlSigner = UrlSigner.FromCredential(_credential);

            var signedUrl = await urlSigner.SignAsync(
                bucket: _bucketName,
                objectName: objectName,
                duration: urlDuration,
                httpMethod: HttpMethod.Get);

            _logger.LogInformation("Generated signed URL for object: {ObjectName}, expires in: {Duration}",
                objectName, urlDuration);
            return signedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate signed URL for object: {ObjectName}", objectName);

            // Fallback to direct URL (will only work if bucket/object is public)
            var fallbackUrl = $"{_baseUrl}/{_bucketName}/{objectName}";
            _logger.LogWarning("Using fallback direct URL: {FallbackUrl}", fallbackUrl);
            return fallbackUrl;
        }
    }
    
    // Get signed URL for an existing image file
    public async Task<string> GetSignedImageUrlAsync(string fileName, TimeSpan? duration = null)
    {
        var objectName = fileName.StartsWith("products/") ? fileName : $"products/{fileName}";
        return await GenerateSignedUrlAsync(objectName, duration);
    }

    // Get public URL (only works if object is public)
    public string GetPublicImageUrl(string fileName)
    {
        var objectName = fileName.StartsWith("products/") ? fileName : $"products/{fileName}";
        return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
    }

    // public string GetImageUrl(string fileName)
    // {
    //     var objectName = fileName.StartsWith("products/") ? fileName : $"products/{fileName}";
    //     return $"{_baseUrl}/{_bucketName}/{objectName}";
    // }


    // --- Batch Operations ---
    
    // Upload multiple images and return their URLs (signed URLs with 1 year expiration)
    /// <param name="imageFiles">Dictionary where key is the unique identifier and value is the image file</param>
    /// <returns>Dictionary where key is the identifier and value is the image URL</returns>
    public async Task<Dictionary<string, string>> UploadBatchImagesAsync(Dictionary<string, IFormFile> imageFiles)
    {
        _logger.LogInformation("Starting batch upload for {Count} files", imageFiles.Count);

        var results = new Dictionary<string, string>();
        var uploadTasks = new List<Task>();

        foreach (var kvp in imageFiles)
        {
            var identifier = kvp.Key;
            var file = kvp.Value;

            _logger.LogInformation("Preparing upload for {Identifier}: {FileName}, Size: {Size} bytes, ContentType: {ContentType}", identifier, file.FileName, file.Length, file.ContentType);

            uploadTasks.Add(Task.Run(async () =>
            {
                try
                {
                    // Validate file before upload
                    if (file == null || file.Length == 0)
                    {
                        _logger.LogWarning("File is null or empty for identifier: {Identifier}", identifier);
                        lock (results)
                        {
                            results[identifier] = string.Empty;
                        }
                        return;
                    }
                    
                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var imageUrl = await UploadImageAsync(file, fileName);
                    
                    lock (results)
                    {
                        results[identifier] = imageUrl;
                    }
      
                    _logger.LogInformation("Batch upload SUCCESS - {Identifier} -> URL: {ImageUrl}", 
                    identifier, imageUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Batch upload - Failed to upload image for identifier: {Identifier}", identifier);
                    lock (results)
                    {
                        results[identifier] = string.Empty; // Mark as failed
                    }
                }
            }));
        }

        await Task.WhenAll(uploadTasks);

        var successCount = results.Count(r => !string.IsNullOrEmpty(r.Value));
        _logger.LogInformation("Batch upload completed. Successfully uploaded: {SuccessCount}/{TotalCount}", 
            successCount, results.Count);

        // Log each result for debugging
        foreach (var result in results)
        {
            _logger.LogInformation("Final result - {Key}: {Value}", result.Key, 
                string.IsNullOrEmpty(result.Value) ? "FAILED" : "SUCCESS");
        }
        
        return results;
    }


    // --- Test Connection ---

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var bucket = await _storageClient.GetBucketAsync(_bucketName);
            _logger.LogInformation("Successfully connected to bucket: {BucketName} (Location: {Location})",
                _bucketName, bucket.Location);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to bucket: {BucketName}", _bucketName);
            return false;
        }
    }
}