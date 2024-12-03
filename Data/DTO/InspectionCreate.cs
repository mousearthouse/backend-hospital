using backend_email.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.DTO;

public class InspectionCreate
{
    [Required]
    public required DateTime Date { get; set; }
    [Required]
    [MaxLength(5000)]
    [MinLength(1)]
    public required string Anamnesis { get; set; }
    [Required]
    [MaxLength(5000)]
    [MinLength(1)]
    public required string Complaints { get; set; }
    [Required]
    [MaxLength(5000)]
    [MinLength(1)]
    public required string Treatment { get; set; }
    [Required]
    public required Conclusion Conclusion { get; set; }
    public DateTime? NextVisitDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? PreviousInspectionId { get; set; }
    [Required]
    [MinLength(1)]
    public required List<DiagnosisCreate> Diagnoses { get; set; }
    public List<ConsultationCreate>? Consultations { get; set; }
}
