using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Product : BaseEntity  // Inherit from BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = "unknow";

    [Required]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(20,2)")]
    public decimal Price { get; set; }

    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Required]
    [StringLength(200)]
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

    public int? EventId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Event? Event { get; set; } // Navigation property to Event

    public ICollection<OrderItem>? OrderItems { get; set; } // Collection of OrderItems associated with the product
}
