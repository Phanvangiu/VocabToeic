using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabToeic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationAndPasswordReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationExpiresAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationToken",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailVerified",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetExpiresAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificationExpiresAt",
                table: "users");

            migrationBuilder.DropColumn(
                name: "EmailVerificationToken",
                table: "users");

            migrationBuilder.DropColumn(
                name: "EmailVerified",
                table: "users");

            migrationBuilder.DropColumn(
                name: "PasswordResetExpiresAt",
                table: "users");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "users");
        }
    }
}
