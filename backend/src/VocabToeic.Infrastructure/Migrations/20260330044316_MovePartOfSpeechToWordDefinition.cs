using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabToeic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MovePartOfSpeechToWordDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartOfSpeech",
                table: "words");

            migrationBuilder.AddColumn<string>(
                name: "PartOfSpeech",
                table: "word_definitions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartOfSpeech",
                table: "word_definitions");

            migrationBuilder.AddColumn<string>(
                name: "PartOfSpeech",
                table: "words",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
