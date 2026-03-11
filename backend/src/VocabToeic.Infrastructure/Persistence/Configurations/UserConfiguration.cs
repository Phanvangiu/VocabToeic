using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations
{
  public class UserConfiguration : IEntityTypeConfiguration<User>
  {
    public void Configure(EntityTypeBuilder<User> builder)
    {
      builder.ToTable("users");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.Email)
        .IsRequired()
        .HasMaxLength(256);

      builder.HasIndex(x => x.Email)
        .IsUnique();

      builder.Property(x => x.PasswordHash)
        .IsRequired();

      builder.Property(x => x.DisplayName)
        .IsRequired()
        .HasMaxLength(100);

      builder.Property(x => x.TargetScore);
      builder.Property(x => x.Streak).HasDefaultValue(0);
      builder.Property(x => x.LastStudyDate);
    }
  }
}