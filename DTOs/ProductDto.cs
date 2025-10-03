using System.ComponentModel.DataAnnotations;

public class ProductDto
{
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }
    public int? CouponId { get; set; }
    public int StoreId { get; set; }
    public int? EventId { get; set; }

    // Image file for upload
    public IFormFile? ImageFile { get; set; }
}