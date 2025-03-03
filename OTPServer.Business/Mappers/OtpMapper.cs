using OTPServer.Business.Dtos;
using OTPServer.Domain.Models;

namespace OTPServer.Business.Mappers
{
    public static class OtpMapper
    {
        public static OtpDto MapToDto(this Otp otp)
        {
            return new OtpDto()
            {
                EncryptedCode = otp.EncryptedCode,
                ExpiresAt = otp.ExpiresAt,
            };
        }
    }
}
