using System.ComponentModel.DataAnnotations;


public class ResetPasswordModel : BaseEntity  // Inherit from BaseEntity
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required]
    public string? Token { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }
}