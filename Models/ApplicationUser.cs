using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// using EcommerceAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
    
    public int? StoreId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Store? Store { get; set; } // Navigation property to Store
    
    public ICollection<Location>? Locations { get; set; } // Navigation property to Locations
    public ICollection<CouponUserList>? CouponUserLists { get; set; } // Navigation property to CouponUserLists
    public ICollection<ChatMessage>? SentMessages { get; set; } // Navigation property for messages sent by the user
    public ICollection<ChatMessage>? ReceivedMessages { get; set; } // Navigation property for messages received by the user
    public ICollection<ChatRoom>? CustomerChatRooms { get; set; } // Navigation property to ChatRooms for customers
    public ICollection<ChatRoom>? SellerChatRooms { get; set; } // Navigation property to ChatRooms for sellers
}