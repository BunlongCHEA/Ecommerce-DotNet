using System.ComponentModel.DataAnnotations;

public class LocationRegion
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string? Region { get; set; } // City/Province/State


    public int CountryId { get; set; } // Foreign key to LocationCountry
    public LocationCountry? LocationCountry { get; set; } // Navigation property to LocationCountry

    public ICollection<Location>? Locations { get; set; } // Navigation property to Location
}
