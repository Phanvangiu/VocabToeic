using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class ExerciseResultConfiguration : IEntityTypeConfiguration<ExerciseResult>
{
  public void Configure(EntityTypeBuilder<ExerciseResult> builder)
  {
    builder.ToTable("exercise_results");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Score)
        .IsRequired();

    builder.Property(x => x.TotalQuestions)
        .IsRequired();

    builder.Property(x => x.TimeTakenSec)
        .IsRequired();

    // Lưu Answers dưới dạng JSONB
    builder.Property(x => x.Answers)
        .HasColumnType("jsonb")
        .IsRequired();

    builder.Property(x => x.CompletedAt)
        .IsRequired();

    builder.HasOne(x => x.User)
        .WithMany(x => x.ExerciseResults)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(x => x.Exercise)
        .WithMany(x => x.Results)
        .HasForeignKey(x => x.ExerciseId)
        .OnDelete(DeleteBehavior.Cascade);

    // Index để query lịch sử làm bài của user
    builder.HasIndex(x => new { x.UserId, x.CompletedAt });
  }
}