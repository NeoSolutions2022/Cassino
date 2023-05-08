using Cassino.Core.Authorization;
using Cassino.Core.Extensions;
using Cassino.Domain.Contracts.Repositories;
using Cassino.Domain.Contracts.Repositories.temp;
using Cassino.Infra.Context;
using Cassino.Infra.Repositories;
using Cassino.Infra.Repositories.temp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cassino.Infra;

public static class DependencyInjection
{
    public static void ConfigureDataBase(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddHttpContextAccessor();

        service.AddScoped<IAuthenticatedUser>(sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

            return httpContextAccessor.UsuarioAutenticado()
                ? new AuthenticatedUser(httpContextAccessor)
                : new AuthenticatedUser();
        });

        service.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });
        
        service.AddScoped<BaseApplicationDbContext>(serviceProvider =>
        {
            var autenticatedUser = serviceProvider.GetRequiredService<IAuthenticatedUser>();

            return serviceProvider.GetRequiredService<ApplicationDbContext>();
        });
    }

    public static void ConfigureRepositories(this IServiceCollection service)
    {
        service
            .AddScoped<IAdministradorRepository, AdministradorRepository>()
            .AddScoped<IRendaCasaRepository, RendaCasaRepository>()
            .AddScoped<IUsuarioRepository, UsuarioRepository>();
    }

    public static void UseMigrations(this IApplicationBuilder app, IServiceProvider service)
    {
        using var scope = service.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}