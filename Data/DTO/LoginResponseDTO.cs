using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.DTO;

public class LoginResponseDTO
{
    [Required]
    [MinLength(1)]
    public required string Token { get; set; }
}
