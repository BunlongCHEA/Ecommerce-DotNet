using System.ComponentModel.DataAnnotations;

public class LocationCountry
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string? CountryName { get; set; } // e.g., "United States", "Canada"

    [Required]
    [StringLength(10)]
    public string? CountryCode { get; set; } // e.g., "US", "CA"
    // [Required]
    // public string? Region { get; set; } // City/Province/State

    public ICollection<LocationRegion>? LocationRegions { get; set; } // Navigation property to Location Region
}
