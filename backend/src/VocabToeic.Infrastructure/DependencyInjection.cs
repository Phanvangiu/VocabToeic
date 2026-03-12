using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VocabToeic.Infrastructure.Persistence;
using VocabToeic.Infrastructure.Services;
namespace VocabToeic.Infrastructure
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
      // PostgreSQL + EF Core
      services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
           configuration.GetConnectionString("DefaultConnection"),
                  npgsql => npgsql.MigrationsAssembly(
                      typeof(AppDbContext).Assembly.FullName)
        )
      );
      // Scoped — depends on IConfiguration, new instance per request
      services.AddScoped<IJwtService, JwtService>();

      // Singleton — stateless, one instance for app lifetime
      services.AddSingleton<IPasswordService, PasswordService>();
      return services;
    }
  }

}