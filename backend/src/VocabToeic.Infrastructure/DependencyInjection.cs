using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
      return services;
    }
  }

}