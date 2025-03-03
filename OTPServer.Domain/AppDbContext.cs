using Microsoft.EntityFrameworkCore;
using OTPServer.Domain.Models;

namespace OTPServer.Domain;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Otp> OTPs { get; set; }
    public DbSet<User> Users { get; set; }
}