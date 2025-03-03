using OTPServer.Domain.Models;

namespace OTPServer.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> getByEmail(string email);
        void Create(User user);
        Task SaveChangesAsync();
    }
}
