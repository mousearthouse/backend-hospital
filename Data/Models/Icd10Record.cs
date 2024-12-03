using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.Models;

public class Icd10Record
{
    [Required]
    public required string Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? IdParent { get; set; }
}

