using System.ComponentModel.DataAnnotations;

namespace backend_email.Data.DTO
{
    public class DoctorRegister
    {
        [MinLength(1)]
        [MaxLength(1000)]
        [Required]
        public required string Name { get; set; }
        [MinLength(6)]
        [Required]
        public required string Password { get; set; }
        [EmailAddress]
        [MinLength(1)]
        [Required]
        public required string Email { get; set; }
        public DateTime? Birthday { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public string? Phone { get; set; }
        [Required]
        public required string Speciality { get; set; }
    }
}
