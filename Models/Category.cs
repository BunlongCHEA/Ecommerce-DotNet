using System.ComponentModel.DataAnnotations;

public class Category : BaseEntity  // Inherit from BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string? Name { get; set; }

    public ICollection<Product>? Products { get; set; } // Navigation property to Products

    // public ICollection<SubCategory>? SubCategories { get; set; } // Navigation property to SubCategories
}