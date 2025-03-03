using System.ComponentModel.DataAnnotations;

namespace OTPServer.Domain.Models
{
    public class Otp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string EncryptedCode { get; set; }

        [Required]
        public required int UserId { get; set; }

        [Required]
        public required DateTime ExpiresAt { get; set; }

        [Required]
        public required bool IsValid { get; set; }
    }
}
