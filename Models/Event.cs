using System.ComponentModel.DataAnnotations;

public class Event : BaseEntity  // Inherit from BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string? Name { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 2)]
    public string? ImageUrl { get; set; } // image for the slide

    [Required]
    public DateTimeOffset? StartDate { get; set; }

    [Required]
    public DateTimeOffset? EndDate { get; set; }

    public string? Description { get; set; } // Optional description of the event

    public ICollection<Product>? Products { get; set; } // Navigation property to Products associated with the event
}