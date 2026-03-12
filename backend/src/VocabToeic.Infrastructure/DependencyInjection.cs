using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VocabToeic.Application.Common.Interfaces;
using VocabToeic.Infrastructure.Persistence;
namespace VocabToeic.Infrastructure
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
           configuration.GetConnectionString("DefaultConnection"),
                  npgsql => npgsql.MigrationsAssembly(
                      typeof(AppDbContext).Assembly.FullName)
        )
      );
      // UnitOfWork — Scoped to match DbContext lifetime
      services.AddScoped<IUnitOfWork, UnitOfWork>();

      // Scoped — depends on IConfiguration, new instance per request
      // services.AddScoped<IJwtService, JwtService>();

      // Singleton — stateless, one instance for app lifetime
      // services.AddSingleton<IPasswordService, PasswordService>();
      return services;
    }
  }

}