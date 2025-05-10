using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class CouponUserList
{
    [Key]
    public int Id { get; set; }
    
    public int CouponId { get; set; } // Foreign key to Coupon
    [JsonIgnore]
    [ValidateNever]
    public Coupon? Coupon { get; set; } // Navigation property to Coupon

    public int UserId { get; set; } // Foreign key to ApplicationUser
    [JsonIgnore]
    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; } // Navigation property to ApplicationUser

    public bool IsUsed { get; set; } // Indicates if the coupon has been used by the user
    public DateTime? UsedDate { get; set; } // Date when the coupon was used
    public DateTime? ExpiryDate { get; set; } // Expiry date of the coupon for the user

    public ICollection<Order>? Orders { get; set; } // Collection of Orders associated with the coupon for the user
}
