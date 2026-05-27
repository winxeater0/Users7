using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users7.Application.Interfaces;
using Users7.Infrastructure.Persistence;
using Users7.Infrastructure.Repositories;
using Users7.Infrastructure.Services;

namespace Users7.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<UsersDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IDateProvider, SystemDateProvider>();

        return services;
    }
}
