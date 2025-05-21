using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; } // Quantity of the item in the order

    [Required]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(20,2)")]
    public decimal Price { get; set; } // Price of the item at the time of order

    [Required]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(20,2)")]
    public decimal TotalPrice { get; set; } // Total price for the quantity of items ordered

    public int ProductId { get; set; } // Foreign key to Product
    [JsonIgnore]
    [ValidateNever]
    public Product? Product { get; set; } // Navigation property to Product

    public int OrderId { get; set; } // Foreign key to Order
    [JsonIgnore]
    [ValidateNever]
    public Order? Order { get; set; } // Navigation property to Order
}