﻿using System.Reflection;
using Cassino.Application.Contracts;
using Cassino.Application.Contracts.temp;
using Cassino.Application.Notification;
using Cassino.Application.Services;
using Cassino.Application.Services.temp;
using Cassino.Core.Settings;
using Cassino.Domain.Entities;
using Cassino.Infra;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScottBrady91.AspNetCore.Identity;

namespace Cassino.Application;

public static class DependencyInjection
{
    public static void ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        services.ConfigureDataBase(configuration);
        services.ConfigureRepositories();

        services
            .AddAutoMapper(Assembly.GetExecutingAssembly());
    }
    
    public static void ConfigureServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAdministradorService, AdministradorService>();

        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<INotificator, Notificator>()
            .AddScoped<IRendaCasaService, RendaCasaService>()
            .AddScoped<IUsuarioService, UsuarioService>()
            .AddScoped<ISaldoService, SaldoService>()
            .AddScoped<ISenhaService, SenhaService>()
            .AddScoped<IApostaService, ApostaService>()
            .AddScoped<IUsuarioAuthService, UsuarioAuthService>()
            .AddScoped<ISaqueService, SaqueService>()
            .AddScoped<IUsuarioCarteiraService, UsuarioCarteiraService>()
            .AddScoped<IPasswordHasher<Administrador>, Argon2PasswordHasher<Administrador>>()
            .AddScoped<IPasswordHasher<Usuario>, Argon2PasswordHasher<Usuario>>();
    }
    
    // public static void UseStaticFileConfiguration(this IApplicationBuilder app, IConfiguration configuration)
    // {
    //     var uploadSettings = configuration.GetSection("UploadSettings");
    //
    //     app.UseStaticFiles(new StaticFileOptions
    //     {
    //         FileProvider = new PhysicalFileProvider(publicBasePath),
    //         RequestPath = $"/{EPathAccess.Public.ToDescriptionString()}"
    //     });
    //
    //     app.UseStaticFiles(new StaticFileOptions
    //     {
    //         FileProvider = new PhysicalFileProvider(privateBasePath),
    //         RequestPath = $"/{EPathAccess.Private.ToDescriptionString()}",
    //         OnPrepareResponse = ctx =>
    //         {
    //             if (ctx.Context.User.UsuarioAutenticado()) return;
    //
    //             // respond HTTP 401 Unauthorized.
    //             ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //             ctx.Context.Response.ContentLength = 0;
    //             ctx.Context.Response.Body = Stream.Null;
    //             ctx.Context.Response.Headers.Add("Cache-Control", "no-store");
    //         }
    //     });
    // }
}