using System.ComponentModel.DataAnnotations;
// using EcommerceAPI.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser<int>
{
    [Required]
    [StringLength(100)]
    public string? FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Required]
    [StringLength(10)]
    public string? Gender { get; set; }

    [Required]
    [StringLength(20)]
    public string? Role { get; set; } = "User"; // Default role is User

    public ICollection<Payment>? Payments { get; set; } // Navigation property to Payments
    // public ICollection<Order>? Orders { get; set; } // Navigation property to Orders
    // public ICollection<Shipment>? Shipments { get; set; } // Navigation property to Shipments
    public ICollection<Location>? Locations { get; set; } // Navigation property to Locations
    public ICollection<CouponUserList>? CouponUserLists { get; set; } // Navigation property to CouponUserLists
}