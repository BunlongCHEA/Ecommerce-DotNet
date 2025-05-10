using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class RegisterModel
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Role { get; set; } // Default role is User
    }
}