using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.Models;

public class InspectionComment
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public string? ParentId { get; set; }
    public string? Content { get; set; }
    public Doctor? Author { get; set; }
    public DateTime? ModifyTime { get; set; }
}
