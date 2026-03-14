using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;
using VocabToeic.Domain.Enums;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("users");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Email)
        .IsRequired()
        .HasMaxLength(256);

    builder.HasIndex(x => x.Email).IsUnique();

    // Nullable — user đăng nhập Google không có password
    builder.Property(x => x.PasswordHash)
        .HasMaxLength(60)
        .IsRequired(false);

    builder.Property(x => x.DisplayName)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(x => x.AvatarUrl)
        .HasMaxLength(500)
        .IsRequired(false);

    // Lưu enum dạng string ("User" / "Admin")
    builder.Property(x => x.Role)
        .HasConversion<string>()
        .HasMaxLength(20)
        .HasDefaultValue(UserRole.User);

    builder.Property(x => x.Streak).HasDefaultValue(0);
    builder.Property(x => x.IsActive).HasDefaultValue(true);

    builder.HasMany(x => x.RefreshTokens)
        .WithOne(r => r.User)
        .HasForeignKey(r => r.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(x => x.WordProgresses)
        .WithOne(p => p.User)
        .HasForeignKey(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(x => x.ExerciseResults)
        .WithOne(r => r.User)
        .HasForeignKey(r => r.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(x => x.StudySessions)
        .WithOne(s => s.User)
        .HasForeignKey(s => s.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(x => x.ListeningProgresses)
        .WithOne(l => l.User)
        .HasForeignKey(l => l.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(x => x.ExternalLogins)
        .WithOne(e => e.User)
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
