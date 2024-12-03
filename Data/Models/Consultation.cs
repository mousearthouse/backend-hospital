using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.Models;

public class Consultation
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public string? InspectionId { get; set; }
    public Speciality? Speciality { get; set; }
    public List<Comment>? Comments { get; set; } = [];
}
