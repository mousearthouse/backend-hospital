using System.ComponentModel.DataAnnotations;

namespace backend_email.Data;

public class DoctorEdit
{
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string Name { get; set; }
    public DateTime? Birthday { get; set; }
    [Required]
    public Gender Gender { get; set; }
    public string? Phone { get; set; }
}