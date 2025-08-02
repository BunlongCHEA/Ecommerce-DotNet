using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class ChatMessage : BaseEntity
{
    [Key]
    public int Id { get; set; }

    public int SenderId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public ApplicationUser? Sender { get; set; } // Navigation property to ApplicationUser

    public int ReceiverId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public ApplicationUser? Receiver { get; set; } // Navigation property to ApplicationUser

    [Required]
    [StringLength(500)]
    public string? Message { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public bool IsRead { get; set; }

    public string? ConnectionType { get; set; } // "customer-seller"

    public int ChatRoomId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public ChatRoom? ChatRoom { get; set; } // Navigation property to ChatRoom
}