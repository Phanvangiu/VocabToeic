using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class UserWordProgressConfiguration : IEntityTypeConfiguration<UserWordProgress>
{
  public void Configure(EntityTypeBuilder<UserWordProgress> builder)
  {
    builder.ToTable("user_word_progresses");

    builder.HasKey(x => x.Id);

    builder.HasIndex(x => new { x.UserId, x.WordId })
        .IsUnique();

    builder.Property(x => x.Status)
        .HasConversion<string>()
        .HasMaxLength(20);

    builder.Property(x => x.EaseFactor)
        .HasDefaultValue(2.5);

    builder.Property(x => x.IntervalDays)
        .HasDefaultValue(1);

    builder.Property(x => x.CorrectCount)
        .HasDefaultValue(0);

    builder.Property(x => x.IncorrectCount)
        .HasDefaultValue(0);

    builder.HasOne(x => x.User)
        .WithMany(x => x.WordProgresses)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(x => x.Word)
        .WithMany(x => x.UserProgresses)
        .HasForeignKey(x => x.WordId)
        .OnDelete(DeleteBehavior.Cascade);

    // Index để query hàng đợi ôn tập nhanh
    builder.HasIndex(x => new { x.UserId, x.NextReviewAt });
  }
}