using System.ComponentModel.DataAnnotations;
using ECommerceAPI.Models;

public class ShipmentType
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? Type { get; set; } // e.g., "Standard", "Express"

    public ICollection<Shipment>? Shipments { get; set; } // Navigation property to Shipments
}