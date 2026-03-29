using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VocabToeic.Infrastructure.Persistence;

/// <summary>
/// Used by EF Core CLI tools (dotnet ef migrations, dotnet ef database update).
/// Bypasses full app startup — no Redis or other services needed.
/// Loads config from .env file.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {
    // Load .env — tìm lên tối đa 5 cấp
    var root = Directory.GetCurrentDirectory();
    for (int i = 0; i < 5; i++)
    {
      var envFile = Path.Combine(root, ".env");
      if (File.Exists(envFile))
      {
        Env.Load(envFile);
        break;
      }
      var parent = Directory.GetParent(root);
      if (parent is null) break;
      root = parent.FullName;
    }

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(),
            "..", "VocabToeic.API"))
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .Build();

    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection is not configured.");

    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    optionsBuilder.UseNpgsql(connectionString,
        npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

    return new AppDbContext(optionsBuilder.Options);
  }
}