using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabToeic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitExerciseContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "exercises");

            migrationBuilder.AddColumn<string>(
                name: "listening_content",
                table: "exercises",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reading_content",
                table: "exercises",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "listening_content",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "reading_content",
                table: "exercises");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "exercises",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
