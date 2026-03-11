using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabToeic.Domain.Entities;

namespace VocabToeic.Infrastructure.Persistence.Configurations;

public class WordDefinitionConfiguration : IEntityTypeConfiguration<WordDefinition>
{
  public void Configure(EntityTypeBuilder<WordDefinition> builder)
  {
    builder.ToTable("word_definitions");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.DefinitionEn)
        .IsRequired();

    builder.Property(x => x.DefinitionVi)
        .IsRequired();

    builder.Property(x => x.ExampleEn);
    builder.Property(x => x.ExampleVi);
    builder.Property(x => x.Tips);

    builder.Property(x => x.SortOrder)
        .HasDefaultValue(0);
  }
}