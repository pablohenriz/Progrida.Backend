using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Progrida.Application.Common.Interfaces;
using Progrida.Domain.Interfaces;
using Progrida.Infrastructure.Persistence;
using Progrida.Infrastructure.Repositories;
using Progrida.Infrastructure.Security;

namespace Progrida.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProgridaDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ProgridaDb")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ProgridaDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskSectionRepository, TaskSectionRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
