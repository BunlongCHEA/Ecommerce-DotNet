using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class ChatRoom : BaseEntity
{
    [Key]
    public int Id { get; set; }

    public int CustomerId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public ApplicationUser? Customer { get; set; } // Navigation property to ApplicationUser

    public int SellerId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public ApplicationUser? Seller { get; set; } // Navigation property to ApplicationUser

    public int StoreId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Store? Store { get; set; } // Navigation property to Store

    public DateTimeOffset? LastActivity { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<ChatMessage>? Messages { get; set; } // Collection of ChatMessages associated with the chat room
}