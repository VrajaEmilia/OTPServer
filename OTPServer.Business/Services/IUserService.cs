using OTPServer.Business.Dtos;

namespace OTPServer.Business.Services
{
    public interface IUserService
    {
        Task<UserDto?> Create(UserDto userDto);
        Task<UserDto?> GetByEmail(string email);
    }
}
