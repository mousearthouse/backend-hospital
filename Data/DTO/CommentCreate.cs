using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_email.Data;

public class CommentCreate
{
    [MaxLength(1000)]
    [MinLength(1)]
    [Required]
    public string Content { get; set; }
    [ForeignKey("Consultation")]
    public Guid ParentId { get; set; }
}
