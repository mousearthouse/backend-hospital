using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.DTO
{
    public class LoginRequestDTO
    {
        [EmailAddress]
        [Required]
        [MinLength(1)]
        public required string Email { get; set; }
        [Required]
        [MinLength(1)]
        public required string Password { get; set; }
    }
}
