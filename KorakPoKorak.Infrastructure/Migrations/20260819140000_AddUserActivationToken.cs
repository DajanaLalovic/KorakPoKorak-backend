using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KorakPoKorak.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserActivationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivationToken",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivationTokenExpires",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ActivationToken",
                table: "Users",
                column: "ActivationToken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ActivationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ActivationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ActivationTokenExpires",
                table: "Users");
        }
    }
}
