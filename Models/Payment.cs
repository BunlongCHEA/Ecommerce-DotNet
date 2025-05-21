using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string? PaymentMethod { get; set; } // e.g., "Credit Card", "PayPal"

    [Required]
    [StringLength(100)]
    public string? BankName { get; set; } // Name of the bank or payment provider

    [Required]
    [StringLength(50)]
    public string? AccountOrCardNumber { get; set; } // Account or Card number or payment identifier

    [Required]
    [DataType(DataType.Date)]
    public DateTimeOffset? CardExpireDate { get; set; } // Expiration date for card payments

    [Required]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(20,2)")]
    public decimal Balance { get; set; } // Balance available in the payment method

    public int UserId { get; set; } // Foreign key to ApplicationUser
    [JsonIgnore]
    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; } // Navigation property to ApplicationUser

    public ICollection<Order>? Orders { get; set; } // Collection of Orders associated with the payment
}