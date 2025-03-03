using OTPServer.Business.Dtos;
using OTPServer.Domain.Models;

namespace OTPServer.Business.Mappers
{
    public static class UserMapper
    {
        public static UserDto MapToDto(this User user)
        {
            return new UserDto()
            {
                Id = user.Id,
                Email = user.Email
            };
        }

        public static User MapToModel(this UserDto userDto)
        {
            return new User()
            {
                Id = userDto.Id,
                Email = userDto.Email
            };
        }
    }
}
