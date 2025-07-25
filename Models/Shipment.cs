using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
// using EcommerceAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Shipment : BaseEntity  // Inherit from BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? TrackingNumber { get; set; } // Unique tracking number for the shipment

    public DateTimeOffset? ShipmentDate { get; set; } // Date when the shipment was created

    public DateTimeOffset? ExpectedDate { get; set; } // Expected date of shipment

    public DateTimeOffset? DelayedDateFrom { get; set; } // Date from which the shipment is delayed

    public DateTimeOffset? ArrivedDate { get; set; } // Actual arrived date

    [Required]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(20,2)")]
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
