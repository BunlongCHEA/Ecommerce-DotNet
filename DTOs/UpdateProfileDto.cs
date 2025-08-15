using System.ComponentModel.DataAnnotations;

public class UpdateProfileDto
{
    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }
}