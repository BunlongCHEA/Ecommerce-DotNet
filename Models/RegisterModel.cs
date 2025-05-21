using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class RegisterModel
    {
        [Required]
        [StringLength(100)]
        public string? UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(10)]
        public string? Gender { get; set; }

        [Required]
        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string? Role { get; set; } // Default role is User
    }
}