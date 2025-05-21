using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? OrderNumber { get; set; } // Unique order number
    
    [Required]
    public DateTimeOffset? OrderDate { get; set; } // Date when the order was placed

    [Required]
    [StringLength(30)]
    public string? Status { get; set; } // e.g., "Pending", "Completed", "Cancelled"

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(20,2)")]
    public Decimal? TotalFinalAmount { get; set; } // Amount after applying any discounts

    [Required]
    [Range(0, int.MaxValue)]
    public int TotalQuantity { get; set; } // Total quantity of items in the order

    [Required]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(20,2)")]
    public decimal TotalAmount { get; set; } // Total amount of the order

    public DateTimeOffset? CancelledDate { get; set; } // Date when the order was cancelled

    public DateTimeOffset? CompletedDate { get; set; } // Date when the order was completed

    public DateTimeOffset? PackedDate { get; set; } // Date when the order was packed

    public int ShipmentId { get; set; } // Foreign key to Shipment
    [JsonIgnore]
    [ValidateNever]
    public Shipment? Shipment { get; set; } // Navigation property to Shipment

    public int PaymentId { get; set; } // Foreign key to Payment
    [JsonIgnore]
    [ValidateNever]
    public Payment? Payment { get; set; } // Navigation property to Payment

    public int? CouponUserListId { get; set; } // Foreign key to CouponUserList
    [JsonIgnore]
    [ValidateNever]
    public CouponUserList? CouponUserList { get; set; } // Navigation property to CouponUserList

    // public int UserId { get; set; } // Foreign key to ApplicationUser
    // [JsonIgnore]
    // [ValidateNever]
    // public ApplicationUser? ApplicationUser { get; set; } // Navigation property to ApplicationUser

    public ICollection<OrderItem>? OrderItems { get; set; } // Collection of OrderItems associated with the order
}
