using Microsoft.EntityFrameworkCore;
using OTPServer.Domain.Models;

namespace OTPServer.Domain.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly AppDbContext _db;

        public OtpRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Create(Otp otp)
        {
            _db.OTPs.Add(otp);
        }

        public async Task<Otp?> GetValidOtpByUserId(int userId)
        {
            return await _db.OTPs
            .Where(otp => otp.UserId == userId && otp.IsValid && otp.ExpiresAt > DateTime.Now)
            .FirstOrDefaultAsync();
        }

        public async Task InvalidateExistingOtps(int userId)
        {
            var existingOtps = await _db.OTPs.Where(o => o.UserId == userId).ToListAsync();
            foreach (var otp in existingOtps)
            {
                otp.IsValid = false;
            }
        }

        public void InvalidateOtp(Otp otp)
        {
            otp.IsValid = false;

        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
