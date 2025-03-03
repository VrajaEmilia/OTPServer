using Microsoft.EntityFrameworkCore;
using OTPServer.Domain.Models;

namespace OTPServer.Domain.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            this._db = db;
        }

        public async Task<User?> getByEmail(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public void Create(User user)
        {
            _db.Users.Add(user);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}

