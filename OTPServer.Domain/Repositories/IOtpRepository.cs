using OTPServer.Domain.Models;

namespace OTPServer.Domain.Repositories
{
    public interface IOtpRepository
    {
        void Create(Otp otp);
        Task<Otp?> GetValidOtpByUserId(int userId);
        Task InvalidateExistingOtps(int userId);
        void InvalidateOtp(Otp otp);
        Task SaveChangesAsync();
    }
}
