using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OTPServer.Domain.Repositories;

namespace OTPServer.Domain
{
    public static class DependencyInjection
    {
        public static void AddDomainDependecies(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("otpDB");
            });
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IOtpRepository, OtpRepository>();
        }
    }
}
