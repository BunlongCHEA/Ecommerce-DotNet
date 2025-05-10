using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
// using EcommerceAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Shipment
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string? TrackingNumber { get; set; } // Unique tracking number for the shipment
    public DateTime? ShipmentDate { get; set; } // Date when the shipment was created
    public DateTime? ExpectedDate { get; set; } // Expected date of shipment
    public DateTime? DelayedDateFrom { get; set; } // Date from which the shipment is delayed
    public DateTime? ArrivedDate { get; set; } // Actual arrived date
    [Required]
    public decimal ShippingCost { get; set; } // Cost of shipping
    
    public int ShipmentTypeId { get; set; } // Foreign key to ShipmentType
    [JsonIgnore]
    [ValidateNever]
    public ShipmentType? ShipmentType { get; set; } // Navigation property to ShipmentType

    public int LocationId { get; set; } // Foreign key to Location
    [JsonIgnore]
    [ValidateNever]
    public Location? Location { get; set; } // Navigation property to Location

    // public int UserId { get; set; } // Foreign key to ApplicationUser
    // [JsonIgnore]
    // [ValidateNever]
    // public ApplicationUser? ApplicationUser { get; set; } // Navigation property to ApplicationUser

    public Order? Order { get; set; } // One-to-one navigation property to Order
    // public ICollection<Order>? Orders { get; set; } // Collection of Orders associated with the shipment
}
