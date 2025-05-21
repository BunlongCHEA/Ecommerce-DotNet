using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Coupon
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

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Product>? Products { get; set; } // Navigation property to Products
    public ICollection<CouponUserList>? CouponUserLists { get; set; } // Navigation property to CouponUserLists
}
