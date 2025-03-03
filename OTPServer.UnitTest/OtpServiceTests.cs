using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OTPServer.Business.Services;
using OTPServer.Domain.Models;
using OTPServer.Domain.Repositories;

namespace OTPServer.UnitTest;
public class OtpServiceTests
{
    private readonly Mock<IOtpRepository> _otpRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<OtpService>> _loggerMock;

    private readonly OtpService _otpService;

    public OtpServiceTests()
    {
        _otpRepositoryMock = new Mock<IOtpRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<OtpService>>();

        var mockSettings = new Dictionary<string, string?>()
        {
            { "SecretKey", "ecpNIu3crVWc26dobWwDrvb5R7q7UGqh" },
            { "OtpLength", "6" },
            { "OtpExpiryTime", "1" }
        };
        var newConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(mockSettings)
            .Build();

        _otpService = new OtpService(_otpRepositoryMock.Object, _userRepositoryMock.Object, _loggerMock.Object, newConfiguration);
    }

    [Fact]
    public async Task GenerateOtp_Success()
    {
        var userModel = new User()
        {
            Id = 1,
            Email = "test@test.test"
        };
        _userRepositoryMock.Setup(x => x.getByEmail(It.IsAny<string>()))
                           .ReturnsAsync(userModel);
        _otpRepositoryMock.Setup(x => x.InvalidateExistingOtps(It.IsAny<int>()))
                          .Returns(Task.CompletedTask);
        _otpRepositoryMock.Setup(x => x.Create(It.IsAny<Otp>()));
        _otpRepositoryMock.Setup(x => x.SaveChangesAsync())
                          .Returns(Task.CompletedTask);

        var code = await _otpService.GenerateOtp(userModel.Id);

        Assert.Equal(44, code?.EncryptedCode.Length);
    }

    [Fact]
    public async Task GenerateOtp_Throws()
    {

        _otpRepositoryMock.Setup(x => x.InvalidateExistingOtps(It.IsAny<int>()))
                          .ThrowsAsync(new Exception());

        await Assert.ThrowsAsync<Exception>(() => _otpService.GenerateOtp(1));
    }

    [Fact]
    public async Task ValidateOtp_Success()
    {
        var email = "myemail@email.com";
        var encrypted = "ZWNwTkl1M2NyVldjMjZkb6fayIwf0h3mDfCcYbdu0vA=";
        var decrypted = "784541";
        var userModel = new User()
        {
            Id = 1,
            Email = email
        };
        var otpModel = new Otp()
        {
            Id = 1,
            EncryptedCode = encrypted,
            ExpiresAt = DateTime.Now.AddSeconds(30),
            IsValid = true,
            UserId = 1
        };
        _userRepositoryMock.Setup(x => x.getByEmail(It.IsAny<string>()))
                           .ReturnsAsync(userModel);
        _otpRepositoryMock.Setup(x => x.GetValidOtpByUserId(It.IsAny<int>()))
                           .ReturnsAsync(otpModel);

        var (isValid, message) = await _otpService.Validate(email, decrypted);

        Assert.True(isValid);
        Assert.Equal("Code validation is successful.", message);
    }

    [Fact]
    public async Task ValidateOtp_Throws()
    {
        // not matching code
        var code = "123456";
        var email = "myemail@email.com";
        var userModel = new User()
        {
            Id = 1,
            Email = email
        };
        var otpModel = new Otp()
        {
            Id = 1,
            EncryptedCode = "ZWNwTkl1M2NyVldjMjZkbw==",
            ExpiresAt = DateTime.UtcNow.AddSeconds(10),
            IsValid = true,
            UserId = 1
        };
        _userRepositoryMock.Setup(x => x.getByEmail(It.IsAny<string>()))
                           .ThrowsAsync(new Exception());

        await Assert.ThrowsAsync<Exception>(() => _otpService.Validate(email, code));
    }
}