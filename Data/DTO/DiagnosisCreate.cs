using System.ComponentModel.DataAnnotations;

namespace backend_email.Data;

public class DiagnosisCreate
{
    [Required]
    public required string IcdDiagnosisId { get; set; }
    [MaxLength(5000)]
    public string? Description { get; set; }
    [Required]
    public DiagnosisType Type { get; set; }
}
