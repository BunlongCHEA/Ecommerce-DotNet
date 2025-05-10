using System.ComponentModel.DataAnnotations;

public class Category
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }

    public ICollection<Product>? Products { get; set; } // Navigation property to Products

    // public ICollection<SubCategory>? SubCategories { get; set; } // Navigation property to SubCategories
}