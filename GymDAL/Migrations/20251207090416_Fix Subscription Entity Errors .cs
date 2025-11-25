using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDAL.Migrations
{
    /// <inheritdoc />
    public partial class FixSubscriptionEntityErrors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_DietPlanAssignments_DietPlanAssignmentId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_WorkoutAssignments_WorkoutAssignmentId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_DietPlanAssignmentId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_WorkoutAssignmentId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "DietPlanAssignmentId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "WorkoutAssignmentId",
                table: "Memberships");

            migrationBuilder.AddColumn<int>(
                name: "DietPlanAssignmentId",
                table: "Subscription",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkoutAssignmentId",
                table: "Subscription",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_DietPlanAssignmentId",
                table: "Subscription",
                column: "DietPlanAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_WorkoutAssignmentId",
                table: "Subscription",
                column: "WorkoutAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_DietPlanAssignments_DietPlanAssignmentId",
                table: "Subscription",
                column: "DietPlanAssignmentId",
                principalTable: "DietPlanAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_WorkoutAssignments_WorkoutAssignmentId",
                table: "Subscription",
                column: "WorkoutAssignmentId",
                principalTable: "WorkoutAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_DietPlanAssignments_DietPlanAssignmentId",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_WorkoutAssignments_WorkoutAssignmentId",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_DietPlanAssignmentId",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_WorkoutAssignmentId",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "DietPlanAssignmentId",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "WorkoutAssignmentId",
                table: "Subscription");

            migrationBuilder.AddColumn<int>(
                name: "DietPlanAssignmentId",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkoutAssignmentId",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_DietPlanAssignmentId",
                table: "Memberships",
                column: "DietPlanAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_WorkoutAssignmentId",
                table: "Memberships",
                column: "WorkoutAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_DietPlanAssignments_DietPlanAssignmentId",
                table: "Memberships",
                column: "DietPlanAssignmentId",
                principalTable: "DietPlanAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_WorkoutAssignments_WorkoutAssignmentId",
                table: "Memberships",
                column: "WorkoutAssignmentId",
                principalTable: "WorkoutAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
