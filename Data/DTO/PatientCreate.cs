using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.DTO;

public class CreatePatient
{
    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public required string Name { get; set; }
    public DateTime? Birthday { get; set; }
    [Required]
    public Gender Gender { get; set; }
}
