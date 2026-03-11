using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations
{
  public class WordConfiguration : IEntityTypeConfiguration<Word>
  {
    public void Configure(EntityTypeBuilder<Word> builder)
    {
      builder.ToTable("words");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.Term)
        .IsRequired()
        .HasMaxLength(100);

      builder.HasIndex(x => x.Term)
        .IsUnique();

      builder.Property(x => x.Phonetic)
        .HasMaxLength(100);

      builder.Property(x => x.AudioUrl);
      builder.Property(x => x.PartOfSpeech)
        .HasMaxLength(20);

      builder.Property(x => x.Topic)
        .IsRequired()
        .HasMaxLength(50);

      builder.Property(x => x.Level)
        .HasDefaultValue(1);

      builder.HasMany(x => x.Definitions)
        .WithOne(x => x.Word)
        .HasForeignKey(x => x.WordId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}