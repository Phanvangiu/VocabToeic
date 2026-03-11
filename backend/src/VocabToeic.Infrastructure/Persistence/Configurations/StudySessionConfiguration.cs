using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class StudySessionConfiguration : IEntityTypeConfiguration<StudySession>
{
  public void Configure(EntityTypeBuilder<StudySession> builder)
  {
    builder.ToTable("study_sessions");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.StartedAt)
        .IsRequired();

    builder.Property(x => x.EndedAt);

    builder.Property(x => x.WordsReviewed)
        .HasDefaultValue(0);

    builder.Property(x => x.ExercisesDone)
        .HasDefaultValue(0);

    builder.HasOne(x => x.User)
        .WithMany(x => x.StudySessions)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // Index để query lịch sử phiên học của user
    builder.HasIndex(x => new { x.UserId, x.StartedAt });
  }
}