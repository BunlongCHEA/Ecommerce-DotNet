using System.ComponentModel.DataAnnotations;

public class LocationCountry
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string? CountryName { get; set; } // e.g., "United States", "Canada"
    public string? CountryCode { get; set; } // e.g., "US", "CA"
    // [Required]
    // public string? Region { get; set; } // City/Province/State

    public ICollection<LocationRegion>? LocationRegions { get; set; } // Navigation property to Location Region
}
