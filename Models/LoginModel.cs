using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class LoginModel
    {
        [Required]
        [StringLength(100)]
        public string? UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string? Password { get; set; }
    }
}