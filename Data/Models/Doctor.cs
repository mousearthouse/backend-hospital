using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.Models;
public class Doctor
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    [MinLength(1)]
    [Required]
    public required string Name { get; set; }
    public DateTime? Birthday { get; set; }
    [Required]
    public required Gender Gender { get; set; }
    [EmailAddress]
    [MinLength(1)]
    [Required]
    public required string Email { get; set; }
    public string? Phone { get; set; }
}
