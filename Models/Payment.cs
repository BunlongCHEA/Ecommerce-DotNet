using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Payment
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string? PaymentMethod { get; set; } // e.g., "Credit Card", "PayPal"
    [Required]
    public string? BankName { get; set; } // Name of the bank or payment provider
    [Required]
    public string? AccountOrCardNumber { get; set; } // Account or Card number or payment identifier
    [Required]
    [DataType(DataType.Date)]
    public DateTime? CardExpireDate { get; set; } // Expiration date for card payments
    [Required]
    public decimal Balance { get; set; } // Balance available in the payment method

    public int UserId { get; set; } // Foreign key to ApplicationUser
    [JsonIgnore]
    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; } // Navigation property to ApplicationUser

    public ICollection<Order>? Orders { get; set; } // Collection of Orders associated with the payment
}