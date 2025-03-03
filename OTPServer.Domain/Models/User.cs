using System.ComponentModel.DataAnnotations;

namespace OTPServer.Domain.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
