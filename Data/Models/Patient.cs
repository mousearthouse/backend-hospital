using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.Models;

public class Patient
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime {  get; set; } = DateTime.UtcNow;
    [Required]
    [MinLength(1)]
    public required string Name { get; set; }
    public DateTime? Birthday { get; set; }
    [Required]
    public required Gender Gender { get; set; }
}
