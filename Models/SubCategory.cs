using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class SubCategory : BaseEntity  // Inherit from BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string? Name { get; set; }
    
    //// The = null!; tells the compiler "trust me, I'll assign this before use" — safe if always set it in code or via EF. Navigation property to Category
    // public int CategoryId { get; set; } // Foreign key to Category
    //// [ForeignKey("CategoryId")]
    //// [JsonIgnore]
    // public Category? Category { get; set; }

    public ICollection<Product>? Products { get; set; } // Navigation property to Products
}