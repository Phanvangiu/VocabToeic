using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class ExternalLoginConfiguration : IEntityTypeConfiguration<ExternalLogin>
{
  public void Configure(EntityTypeBuilder<ExternalLogin> builder)
  {
    builder.ToTable("external_logins");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Provider)
    .HasConversion<string>()
    .HasMaxLength(20)
    .IsRequired();

    builder.Property(x => x.ProviderKey)
    .HasMaxLength(256)
    .IsRequired();

    builder.Property(x => x.DisplayName)
    .HasMaxLength(100);

    builder.Property(x => x.AvatarUrl)
    .HasMaxLength(500);

    builder.HasIndex(x => new { x.Provider, x.ProviderKey }).IsUnique();

    builder.HasIndex(x => x.UserId);

    builder.HasOne(x => x.User)
    .WithMany(u => u.ExternalLogins)
    .HasForeignKey(x => x.UserId)
    .OnDelete(DeleteBehavior.Cascade);
  }
}