using System.ComponentModel.DataAnnotations;

public class ForgotPasswordModel : BaseEntity  // Inherit from BaseEntity
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    
    public string? ClientUrl { get; set; } // The frontend URL to go to reset form
}