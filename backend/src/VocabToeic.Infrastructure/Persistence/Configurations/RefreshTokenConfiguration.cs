using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.ToTable("refresh_tokens");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Token)
        .IsRequired();

    builder.HasIndex(x => x.Token)
        .IsUnique();

    builder.Property(x => x.DeviceInfo)
        .HasMaxLength(256);

    builder.Property(x => x.IpAddress)
        .HasMaxLength(45);

    builder.Property(x => x.ExpiresAt)
        .IsRequired();

    builder.Property(x => x.RevokedAt);

    builder.Property(x => x.RevokedReason)
        .HasMaxLength(100);

    // Ignore computed property
    builder.Ignore(x => x.IsActive);

    builder.HasOne(x => x.User)
        .WithMany(x => x.RefreshTokens)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // Index để query token còn hiệu lực của user
    builder.HasIndex(x => new { x.UserId, x.ExpiresAt });
  }
}