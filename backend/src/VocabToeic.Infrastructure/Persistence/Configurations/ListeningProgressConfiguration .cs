using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class ListeningProgressConfiguration : IEntityTypeConfiguration<ListeningProgress>
{
  public void Configure(EntityTypeBuilder<ListeningProgress> builder)
  {
    builder.ToTable("listening_progresses");

    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.UserId);

    builder.HasIndex(x => x.ExerciseId);

    builder.HasIndex(x => new { x.UserId, x.ExerciseId }).IsUnique();

    builder.Property(x => x.ListenedCount).HasDefaultValue(0);
    builder.Property(x => x.IsCompleted).HasDefaultValue(false);
    builder.Property(x => x.LastListenedAt).IsRequired(false);

    builder.HasOne(x => x.User)
        .WithMany(u => u.ListeningProgresses)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(x => x.Exercise)
        .WithMany(e => e.ListeningProgresses)
        .HasForeignKey(x => x.ExerciseId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}