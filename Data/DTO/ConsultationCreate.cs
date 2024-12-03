using System.ComponentModel.DataAnnotations;

namespace backend_email.Data;

public class ConsultationCreate
{
    [Required]
    public required string SpecialityId { get; set; }
    [Required]
    public required CommentCreate Comment { get; set; }
}
