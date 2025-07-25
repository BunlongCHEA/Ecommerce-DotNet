using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Location : BaseEntity  // Inherit from BaseEntity
{
    [Key]
    public int Id { get; set; }

    [StringLength(50, MinimumLength = 2)]
    public string? PostalCode { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string? Address { get; set; }

    public int RegionId { get; set; } // Foreign key to LocationRegion
    [JsonIgnore]
    [ValidateNever]
    public LocationRegion? LocationRegion { get; set; } // Navigation property to LocationRegion

    // public int CountryId { get; set; } // Foreign key to LocationCountry
    // [JsonIgnore]
    // [ValidateNever]
    // public LocationCountry? LocationCountry { get; set; } // Navigation property to LocationCountry

    public int UserId { get; set; } // Foreign key to ApplicationUser
    [JsonIgnore]
    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; } // Navigation property to ApplicationUser

    public ICollection<Shipment>? Shipments { get; set; } // Collection of shipments associated with the location
}
