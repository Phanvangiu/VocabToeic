using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public void Configure(EntityTypeBuilder<Exercise> builder)
  {
    builder.ToTable("exercises");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Part)
        .HasConversion<int>();

    builder.Property(x => x.Topic)
        .IsRequired()
        .HasMaxLength(50);

    builder.Property(x => x.Difficulty)
        .HasDefaultValue(1);

    builder.Property(x => x.IsAiGenerated)
        .HasDefaultValue(true);

    // Cột JSONB cho Reading (Part 5-7) — nullable vì Listening không dùng
    builder.Property(x => x.ReadingContent)
        .HasColumnName("reading_content")
        .HasColumnType("jsonb")
        .IsRequired(false)
        .HasConversion(
            v => JsonSerializer.Serialize(v, _jsonOptions),
            v => JsonSerializer.Deserialize<ReadingContent>(v, _jsonOptions));

    // Cột JSONB cho Listening (Part 1-4) — nullable vì Reading không dùng
    builder.Property(x => x.ListeningContent)
        .HasColumnName("listening_content")
        .HasColumnType("jsonb")
        .IsRequired(false)
        .HasConversion(
            v => JsonSerializer.Serialize(v, _jsonOptions),
            v => JsonSerializer.Deserialize<ListeningContent>(v, _jsonOptions));

    builder.HasMany(x => x.Results)
        .WithOne(r => r.Exercise)
        .HasForeignKey(r => r.ExerciseId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(x => x.ListeningProgresses)
        .WithOne(l => l.Exercise)
        .HasForeignKey(l => l.ExerciseId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}