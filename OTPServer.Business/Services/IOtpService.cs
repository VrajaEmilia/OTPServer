using OTPServer.Business.Dtos;

namespace OTPServer.Business.Services
{
    public interface IOtpService
    {
        Task<OtpDto?> GenerateOtp(int userId);
        Task<(bool, string)> Validate(string email, string code);
    }
}
