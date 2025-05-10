using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = "unknow";
    [Required]
    public decimal Price { get; set; }
    public string? Description { get; set; } = "";
    [Required]
    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Category? Category { get; set; } // Navigation property to Category

    public int SubCategoryId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public SubCategory? SubCategory { get; set; }  // Navigation property to SubCategory

    public int? CouponId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Coupon? Coupon { get; set; } // Navigation property to Coupon

    public int StoreId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Store? Store { get; set; }   // Navigation property to Store

    public ICollection<OrderItem>? OrderItems { get; set; } // Collection of OrderItems associated with the product
}
