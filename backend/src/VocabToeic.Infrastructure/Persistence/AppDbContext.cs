using Microsoft.EntityFrameworkCore;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<User> Users => Set<User>();
  public DbSet<Word> Words => Set<Word>();
  public DbSet<WordDefinition> WordDefinitions => Set<WordDefinition>();
  public DbSet<UserWordProgress> UserWordProgresses => Set<UserWordProgress>();
  public DbSet<Exercise> Exercises => Set<Exercise>();
  public DbSet<ExerciseResult> ExerciseResults => Set<ExerciseResult>();
  public DbSet<StudySession> StudySessions => Set<StudySession>();
  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
  public DbSet<ListeningProgress> ListeningProgresses => Set<ListeningProgress>();
  public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    base.OnModelCreating(modelBuilder);
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    foreach (var entry in ChangeTracker.Entries<BaseEntity>())
    {
      if (entry.State == EntityState.Modified)
        entry.Entity.UpdatedAt = DateTime.UtcNow;
    }
    return base.SaveChangesAsync(cancellationToken);
  }
}