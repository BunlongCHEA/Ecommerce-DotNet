using System.ComponentModel.DataAnnotations;

public class Coupon
{
    [Key]
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Type { get; set; } // e.g., "Store", "User"
    public decimal DiscountAmount { get; set; }
    public string? Desription { get; set; } // Description of the coupon
    public bool IsActive { get; set; } // Indicates if the coupon is active
    public DateTime? StartDate { get; set; } // Start date of the coupon validity
    public int DurationValidity { get; set; } // Duration of validity in days

    public ICollection<Product>? Products { get; set; } // Navigation property to Products
    public ICollection<CouponUserList>? CouponUserLists { get; set; } // Navigation property to CouponUserLists
}
