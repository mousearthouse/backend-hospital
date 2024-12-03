using System.ComponentModel.DataAnnotations;

namespace backend_email.Data
{
    public class InspectionCommentCreate
    {
        [Required]
        public required string Content { get; set; }
    }
}
