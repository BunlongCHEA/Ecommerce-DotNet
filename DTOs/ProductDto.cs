using System.ComponentModel.DataAnnotations;

public class ProductDto
{
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    // For JSON requests - base64 image data
    public string? ImageBase64 { get; set; }
    public string? ImageFileName { get; set; }
    public string? ImageContentType { get; set; }

    // Image file for upload
    public IFormFile? ImageFile { get; set; }

    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }
    public int? CouponId { get; set; }
    public int StoreId { get; set; }
    // public int? EventId { get; set; }

    // Helper method to convert base64 to IFormFile
    public IFormFile? GetImageFile()
    {
        if (ImageFile != null) return ImageFile; // FormData case
        
        if (string.IsNullOrEmpty(ImageBase64)) return null;

        try
        {
            // Remove data:image/jpeg;base64, prefix if present
            var base64Data = ImageBase64;
            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Split(',')[1];
            }

            var imageBytes = Convert.FromBase64String(base64Data);
            var stream = new MemoryStream(imageBytes);
            
            return new FormFile(stream, 0, imageBytes.Length, "image", 
                ImageFileName ?? "image.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = ImageContentType ?? "image/jpeg"
            };
        }
        catch
        {
            return null;
        }
    }
}