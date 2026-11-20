using Auth.Service.Application.Interfaces;
using Auth.Service.Application.UseCases;
using Auth.Service.Infrastructure.Persistence;
using Auth.Service.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Service.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AuthDbContext>(o =>
            o.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ILoginUseCase, LoginUseCase>();

        return services;
    }
}
