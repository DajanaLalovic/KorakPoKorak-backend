using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KorakPoKorak.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkshopCreatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Workshops",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Workshops_CreatedById",
                table: "Workshops",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Workshops_Users_CreatedById",
                table: "Workshops",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workshops_Users_CreatedById",
                table: "Workshops");

            migrationBuilder.DropIndex(
                name: "IX_Workshops_CreatedById",
                table: "Workshops");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Workshops");
        }
    }
}
