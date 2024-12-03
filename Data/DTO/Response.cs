using System.ComponentModel.DataAnnotations;

namespace backend_email.Data;
public class Response
{
    [Key]
    public Guid Id { get; set; }
    public string? Status { get; set; }
    public string? Message { get; set; }
}