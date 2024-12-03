using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.Models;
public class Diagnosis
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    [Required]
    [MinLength(1)]
    public required string Code { get; set; }
    [Required]
    [MinLength(1)]
    public required string Name { get; set; }
    [MinLength(1)]
    public string? Description { get; set; }
    [Required]
    public required DiagnosisType Type { get; set; }
}