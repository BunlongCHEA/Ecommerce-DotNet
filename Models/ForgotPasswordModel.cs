using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        
        public string? ClientUrl { get; set; } // The frontend URL to go to reset form
    }
}