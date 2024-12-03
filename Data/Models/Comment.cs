using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.Models;
public class Comment
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    [Required]
    public required string Content { get; set; }
    [Required]
    public required Guid AuthorId { get; set; }
    [Required]
    [MinLength(1)]
    public required string Author {  get; set; }
    public Guid? ParentId { get; set; }
}

