using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KorakPoKorak.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuizId",
                table: "Exercises",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_QuizId",
                table: "Exercises",
                column: "QuizId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_Quizzes_QuizId",
                table: "Exercises",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_Quizzes_QuizId",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_QuizId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "Exercises");
        }
    }
}
