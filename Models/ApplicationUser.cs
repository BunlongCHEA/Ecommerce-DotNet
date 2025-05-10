using System.ComponentModel.DataAnnotations;
// using EcommerceAPI.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser<int>
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    public string? Gender { get; set; }
    [Required]
    public string? Role { get; set; } = "User"; // Default role is User

    public ICollection<Payment>? Payments { get; set; } // Navigation property to Payments
    // public ICollection<Order>? Orders { get; set; } // Navigation property to Orders
    // public ICollection<Shipment>? Shipments { get; set; } // Navigation property to Shipments
    public ICollection<Location>? Locations { get; set; } // Navigation property to Locations
    public ICollection<CouponUserList>? CouponUserLists { get; set; } // Navigation property to CouponUserLists
}