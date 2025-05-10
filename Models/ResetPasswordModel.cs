using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Token { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string? NewPassword { get; set; }
    }
}