using System.ComponentModel.DataAnnotations;

public class Store
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }

    public ICollection<Product>? Products { get; set; } // Navigation property to Products
}