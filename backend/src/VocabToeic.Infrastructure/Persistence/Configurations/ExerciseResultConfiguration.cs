using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class ExerciseResultConfiguration : IEntityTypeConfiguration<ExerciseResult>
{
  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public void Configure(EntityTypeBuilder<ExerciseResult> builder)
  {
    builder.ToTable("exercise_results");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Score).IsRequired();
    builder.Property(x => x.TotalQuestions).IsRequired();
    builder.Property(x => x.TimeTakenSec).IsRequired();
    builder.Property(x => x.CompletedAt).IsRequired();

    // Value converter: Dictionary<string, string> ↔ JSONB
    // {"1": "A", "2": "C", "3": "B"}
    builder.Property(x => x.Answers)
        .HasColumnType("jsonb")
        .IsRequired()
        .HasConversion(
            v => JsonSerializer.Serialize(v, _jsonOptions),
            v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, _jsonOptions)!);

    builder.HasOne(x => x.User)
        .WithMany(x => x.ExerciseResults)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(x => x.Exercise)
        .WithMany(x => x.Results)
        .HasForeignKey(x => x.ExerciseId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasIndex(x => new { x.UserId, x.CompletedAt });
  }
}