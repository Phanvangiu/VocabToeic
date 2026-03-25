using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using VocabToeic.Application.Common.Interfaces;
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
      // UnitOfWork — Scoped to match DbContext lifetime
      services.AddScoped<IUnitOfWork, UnitOfWork>();

      // Scoped — depends on IConfiguration, new instance per request
      services.AddScoped<IJwtService, JwtService>();

      // Singleton — stateless, one instance for app lifetime
      services.AddSingleton<IPasswordService, PasswordService>();


      // Redis
      var redisConnection = configuration.GetConnectionString("Redis")
          ?? throw new InvalidOperationException("Redis connection string is not configured.");
      var redisConfig = ConfigurationOptions.Parse(redisConnection);
      redisConfig.AbortOnConnectFail = false;
      redisConfig.Ssl = true;
      redisConfig.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
      services.AddSingleton<IConnectionMultiplexer>(
          ConnectionMultiplexer.Connect(redisConfig));
      services.AddScoped<IRedisService, RedisService>();
      services.AddScoped<IHealthService, HealthService>();

      services.AddScoped<IGoogleAuthService, GoogleAuthService>();

      services.AddHttpClient();
      services.AddScoped<IEmailService, EmailService>();


      return services;
    }
  }

}