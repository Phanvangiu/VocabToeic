using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
  public void Configure(EntityTypeBuilder<Exercise> builder)
  {
    builder.ToTable("exercises");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Part)
        .HasConversion<int>()
        .IsRequired();

    builder.Property(x => x.Topic)
        .IsRequired()
        .HasMaxLength(50);

    builder.Property(x => x.Difficulty)
        .HasDefaultValue(1);

    builder.Property(x => x.IsAiGenerated)
        .HasDefaultValue(true);

    // Lưu ExerciseContent dưới dạng JSONB
    builder.Property(x => x.Content)
        .HasColumnType("jsonb")
        .IsRequired();

    builder.HasMany(x => x.Results)
        .WithOne(x => x.Exercise)
        .HasForeignKey(x => x.ExerciseId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}