using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabToeic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameWordConfigurationsToWords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_word_progresses_wordConfigurations_WordId",
                table: "user_word_progresses");

            migrationBuilder.DropForeignKey(
                name: "FK_word_definitions_wordConfigurations_WordId",
                table: "word_definitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_wordConfigurations",
                table: "wordConfigurations");

            migrationBuilder.RenameTable(
                name: "wordConfigurations",
                newName: "words");

            migrationBuilder.RenameIndex(
                name: "IX_wordConfigurations_Term",
                table: "words",
                newName: "IX_words_Term");

            migrationBuilder.AddPrimaryKey(
                name: "PK_words",
                table: "words",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_word_progresses_words_WordId",
                table: "user_word_progresses",
                column: "WordId",
                principalTable: "words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_word_definitions_words_WordId",
                table: "word_definitions",
                column: "WordId",
                principalTable: "words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_word_progresses_words_WordId",
                table: "user_word_progresses");

            migrationBuilder.DropForeignKey(
                name: "FK_word_definitions_words_WordId",
                table: "word_definitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_words",
                table: "words");

            migrationBuilder.RenameTable(
                name: "words",
                newName: "wordConfigurations");

            migrationBuilder.RenameIndex(
                name: "IX_words_Term",
                table: "wordConfigurations",
                newName: "IX_wordConfigurations_Term");

            migrationBuilder.AddPrimaryKey(
                name: "PK_wordConfigurations",
                table: "wordConfigurations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_word_progresses_wordConfigurations_WordId",
                table: "user_word_progresses",
                column: "WordId",
                principalTable: "wordConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_word_definitions_wordConfigurations_WordId",
                table: "word_definitions",
                column: "WordId",
                principalTable: "wordConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
