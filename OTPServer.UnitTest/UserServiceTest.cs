using Microsoft.Extensions.Logging;
using Moq;
using OTPServer.Business.Dtos;
using OTPServer.Business.Services;
using OTPServer.Domain.Models;
using OTPServer.Domain.Repositories;

namespace OTPServer.UnitTest;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();

        _userService = new UserService(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Create_Success()
    {
        var email = "test@test.com";
        var userDto = new UserDto { Email = email };

        var existingUser = (User)null;
        _userRepositoryMock.Setup(x => x.getByEmail(email))
                           .ReturnsAsync(existingUser);

        var userModel = new User { Id = 1, Email = email };
        _userRepositoryMock.Setup(x => x.Create(userModel));
        _userRepositoryMock.Setup(x => x.SaveChangesAsync())
                           .Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(x => x.getByEmail(email))
                           .ReturnsAsync(userModel);

        var result = await _userService.Create(userDto);

        Assert.NotNull(result);
        Assert.Equal(userDto.Email, result?.Email);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Create_UserAlreadyExists_ThrowsException()
    {
        var userDto = new UserDto { Email = "test@test.com" };
        var existingUser = new User { Id = 1, Email = "test@test.com" };
        _userRepositoryMock.Setup(x => x.getByEmail(It.IsAny<string>()))
                           .ReturnsAsync(existingUser);

        var exception = await Assert.ThrowsAsync<Exception>(() => _userService.Create(userDto));

        Assert.Equal("User already exists", exception.Message);
        _userRepositoryMock.Verify(x => x.Create(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task GetByEmail_Success()
    {
        var email = "test@test.com";
        var userModel = new User { Id = 1, Email = email };
        _userRepositoryMock.Setup(x => x.getByEmail(It.IsAny<string>()))
                           .ReturnsAsync(userModel);

        var result = await _userService.GetByEmail(email);

        Assert.NotNull(result);
        Assert.Equal(email, result?.Email);
    }

    [Fact]
    public async Task GetByEmail_UserNotFound_ReturnsNull()
    {
        var email = "nonexistent@test.com";
        _userRepositoryMock.Setup(x => x.getByEmail(It.IsAny<string>()))
                           .ReturnsAsync((User)null);

        var result = await _userService.GetByEmail(email);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmail_ThrowsException()
    {
        var email = "test@test.com";
        _userRepositoryMock.Setup(x => x.getByEmail(It.IsAny<string>()))
                           .ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _userService.GetByEmail(email));
    }
}
