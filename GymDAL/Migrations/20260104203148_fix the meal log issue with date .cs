using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDAL.Migrations
{
    /// <inheritdoc />
    public partial class fixthemeallogissuewithdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MealLogs_DietPlanAssignmentId_Date",
                table: "MealLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MealLogs_DietPlanAssignmentId_Date",
                table: "MealLogs",
                columns: new[] { "DietPlanAssignmentId", "Date" },
                unique: true);
        }
    }
}
