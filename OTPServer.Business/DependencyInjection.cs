using Microsoft.Extensions.DependencyInjection;
using OTPServer.Business.Services;
using OTPServer.Domain;

namespace OTPServer.Business;

public static class DependencyInjection
{
    public static void AddBusinessDependencies(this IServiceCollection services)
    {
        services.AddDomainDependecies();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IOtpService, OtpService>();
    }
}
