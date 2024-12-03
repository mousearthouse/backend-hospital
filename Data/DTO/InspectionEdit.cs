using System.ComponentModel.DataAnnotations;

namespace backend_email.Data;

public class InspectionEdit
{
    [Required]
    [MaxLength(5000)]
    [MinLength(1)]
    public string? Anamnesis { get; set; }
    [Required]
    [MaxLength(5000)]
    [MinLength(1)]
    public string? Complaints { get; set; }
    [Required]
    [MaxLength(5000)]
    [MinLength(1)]
    public string? Treatment { get; set; }
    [Required]
    public Conclusion Conclusion { get; set; }
    public DateTime? NextVisitDate { get; set; }
    public DateTime? DeathDate { get; set; }
    [MinLength(1)]
    public List<DiagnosisCreate> Diagnoses { get; set; }
}
