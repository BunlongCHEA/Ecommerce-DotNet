using System.ComponentModel.DataAnnotations;

public class Store : BaseEntity  // Inherit from BaseEntity
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(100)]
    public string? Latitude { get; set; }

    [StringLength(100)]
    public string? Longitude { get; set; }

    public ICollection<Product>? Products { get; set; } // Navigation property to Products
    public ICollection<ChatRoom>? ChatRooms { get; set; } // Navigation property to ChatRooms
    public ICollection<ApplicationUser>? ApplicationUsers { get; set; } // Navigation property to ApplicationUsers
}