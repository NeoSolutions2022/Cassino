using System.Reflection;
using Cassino.Core.Authorization;
using Cassino.Domain.Contracts;
using Cassino.Domain.Entities;
using Cassino.Domain.Entities.temp;
using Cassino.Infra.Converters;
using Cassino.Infra.Extensions;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Cassino.Infra.Context;

public abstract class BaseApplicationDbContext : DbContext, IUnitOfWork
{
    protected readonly IAuthenticatedUser AuthenticatedUser;

    protected BaseApplicationDbContext(DbContextOptions options ,IAuthenticatedUser authenticatedUser) : base(options)
    {
        AuthenticatedUser = authenticatedUser;
    }

    public DbSet<Administrador> Administradores { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Renda> Rendas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
            .UseCollation("utf8mb4_0900_ai_ci");
        
        ApplyConfiguration(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (AuthenticatedUser.UsuarioAdministrador)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Commit() => await SaveChangesAsync() > 0;

        private static void ApplyConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ValidationResult>();
        modelBuilder.ApplyEntityConfiguration();
        modelBuilder.ApplyTrackingConfiguration();
        modelBuilder.ApplySoftDeleteConfiguration();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateOnly>()
            .HaveConversion<DateOnlyCustomConverter>()
            .HaveColumnType("DATE");

        configurationBuilder 
            .Properties<TimeOnly>()
            .HaveConversion<TimeOnlyCustomConverter>()
            .HaveColumnType("TIME");
    }
}