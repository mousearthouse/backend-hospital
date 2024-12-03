using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend_email.Data.Models;

public class Inspection
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public DateTime? Date {  get; set; }
    public string? Anamnesis { get; set; }
    public string? Complaints { get; set; }
    public string? Treatment { get; set; }
    public Conclusion Conclusion { get; set; }
    public DateTime? NextVisitDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? BaseInspectionId { get; set; }
    public string? PreviousInspectionId { get; set; }
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public List<Diagnosis> Diagnoses { get; set; } = [];
    public List<InspectionConsultation> Consultations { get; set; } = [];
    public bool IsNotified { get; set; } = false;

    [JsonIgnore]

    [ForeignKey("Patient")]
    public Guid PatientId { get; set; }

    [ForeignKey("Doctor")]
    public Guid DoctorId { get; set; }

}
