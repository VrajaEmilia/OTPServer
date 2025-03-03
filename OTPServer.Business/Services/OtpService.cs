using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OTPServer.Business.Dtos;
using OTPServer.Business.Mappers;
using OTPServer.Domain.Models;
using OTPServer.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace OTPServer.Business.Services
{
    public class OtpService : IOtpService
    {
        private readonly IOtpRepository _otpRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OtpService> _logger;

        private readonly int _otpLength;
        private readonly int _otpExpiryTime;
        private readonly string _secretKey;

        public OtpService(IOtpRepository otpRepository, IUserRepository userRepository, ILogger<OtpService> logger, IConfiguration configuration)
        {
            _otpRepository = otpRepository;
            _userRepository = userRepository;
            _logger = logger;
            _otpLength = int.Parse(configuration["OtpLength"] ?? throw new ArgumentNullException(nameof(configuration)));
            _otpExpiryTime = int.Parse(configuration["OtpExpiryTime"] ?? throw new ArgumentNullException(nameof(configuration))); ;
            _secretKey = configuration["SecretKey"] ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<OtpDto?> GenerateOtp(int userId)
        {
            try
            {
                await _otpRepository.InvalidateExistingOtps(userId);

                var code = generateCode();

                //For Testing
                _logger.LogInformation($"Generated OTP: {code}");

                var encryptedCode = EncryptCode(code);

                var otp = Create(userId, encryptedCode);

                await _otpRepository.SaveChangesAsync();

                return otp.MapToDto();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating OTP");
                throw;
            }
        }

        private Otp Create(int userId, string encryptedCode)
        {
            var otp = new Otp()
            {
                UserId = userId,
                EncryptedCode = encryptedCode,
                ExpiresAt = DateTime.Now.AddSeconds(_otpExpiryTime),
                IsValid = true
            };

            _otpRepository.Create(otp);

            return otp;
        }

        public async Task<(bool, string)> Validate(string email, string code)
        {
            try
            {
                var user = await _userRepository.getByEmail(email);

                if (user == null)
                {
                    return (false, "User not found");
                }

                var otp = await _otpRepository.GetValidOtpByUserId(user.Id);

                if (otp == null)
                {
                    return (false, "OTP has expired or is incorrect");
                }

                if (otp?.EncryptedCode == EncryptCode(code))
                {
                    _otpRepository.InvalidateOtp(otp);
                    await _otpRepository.SaveChangesAsync();
                    return (true, "Code validation is successful.");
                }

                return (false, "Code validation is unsuccessful"); ;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while validating OTP");
                throw;
            }
        }

        private string generateCode()
        {
            var random = RandomNumberGenerator.Create();
            var randomBytes = new byte[_otpLength];
            random.GetBytes(randomBytes);
            var code = "";

            foreach (byte b in randomBytes)
                code += b % 10;

            return code;
        }

        private string EncryptCode(string code)
        {
            using var aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(_secretKey);  // 32 bytes key for AES-256
            aes.IV = Encoding.UTF8.GetBytes(_secretKey.Substring(0, 16));  // 16 bytes IV

            var encryptor = aes.CreateEncryptor();

            using var memoryStream = new MemoryStream();

            memoryStream.Write(aes.IV, 0, aes.IV.Length);


            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);


            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(code);

            }


            var encryptedBytes = memoryStream.ToArray();
            return Convert.ToBase64String(encryptedBytes);
        }
    }
}

