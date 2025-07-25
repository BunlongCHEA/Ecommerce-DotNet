using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Payment : BaseEntity  // Inherit from BaseEntity
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
    [StringLength(100)]
    public string? CardHolderName { get; set; } // Name of the cardholder or account holder

    [Required]
    [DataType(DataType.Date)]
    public DateTimeOffset? CardExpireDate { get; set; } // Expiration date for card payments

    [Required]
    [Range(100, 999)] // CVV is typically 3 digits for most cards
    [DataType(DataType.Password)] // Use Password type for CVV to hide input
    public int? CVV { get; set; } // CVV for card payments, optional for other methods

    // [Required]
    // [DataType(DataType.Currency)]
    // [Column(TypeName = "decimal(20,2)")]
    // public decimal Balance { get; set; } // Balance available in the payment method

    public int UserId { get; set; } // Foreign key to ApplicationUser
    [JsonIgnore]
    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; } // Navigation property to ApplicationUser

    public ICollection<Order>? Orders { get; set; } // Collection of Orders associated with the payment
}