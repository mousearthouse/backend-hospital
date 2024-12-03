using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.Models;

public class Speciality
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    [Required]
    [MinLength(1)]
    public required string Name { get; set; }
}
