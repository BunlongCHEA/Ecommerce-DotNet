using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class ResetPasswordModel
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
}