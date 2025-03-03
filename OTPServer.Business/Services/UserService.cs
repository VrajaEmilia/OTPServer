using Microsoft.Extensions.Logging;
using OTPServer.Business.Dtos;
using OTPServer.Business.Mappers;
using OTPServer.Domain.Repositories;

namespace OTPServer.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserDto?> Create(UserDto userDto)
        {
            try
            {
                var user = await _userRepository.getByEmail(userDto.Email);
                if (user != null)
                {
                    _logger.LogError("User already exists");
                    throw new Exception("User already exists");
                }
                _userRepository.Create(userDto.MapToModel());
                await _userRepository.SaveChangesAsync();
                _logger.LogInformation("User created");

                user = await _userRepository.getByEmail(userDto.Email);
                return user?.MapToDto();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating user");
                throw;
            }
        }

        public async Task<UserDto?> GetByEmail(string email)
        {
            try
            {
                var user = await _userRepository.getByEmail(email);

                if (user == null)
                {
                    return null;
                }

                return user.MapToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user by email");
                throw;
            }
        }
    }
}

