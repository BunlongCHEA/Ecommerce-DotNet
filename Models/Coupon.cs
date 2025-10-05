using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Coupon : BaseEntity  // Inherit from BaseEntity
{
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    public string? Code { get; set; }

    [StringLength(30)]
    public string? Type { get; set; } // e.g., "Store", "User"

    [Range(0, int.MaxValue)]
    public int DiscountPercentage { get; set; }

    [DataType(DataType.MultilineText)]
    public string? Desription { get; set; } // Description of the coupon

    public bool IsActive { get; set; } // Indicates if the coupon is active

    public DateTimeOffset? StartDate { get; set; } // Start date of the coupon validity

    [Range(0, int.MaxValue)]
    public int DurationValidity { get; set; } // Duration of validity in days

    public int? EventId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Event? Event { get; set; } // Navigation property to Event

    public ICollection<Product>? Products { get; set; } // Navigation property to Products
    public ICollection<CouponUserList>? CouponUserLists { get; set; } // Navigation property to CouponUserLists
}
