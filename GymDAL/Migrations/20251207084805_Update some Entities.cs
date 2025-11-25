using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesomeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanAssignments_Members_MemberId",
                table: "DietPlanAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_DietPlans_DietPlanId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_DietPlans_DietPlanId1",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_WorkoutPlans_WorkoutPlanId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutAssignments_Members_MemberId",
                table: "WorkoutAssignments");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutAssignments_MemberId_IsActive",
                table: "WorkoutAssignments");

            migrationBuilder.DropIndex(
                name: "IX_DietPlanAssignments_MemberId",
                table: "DietPlanAssignments");

            migrationBuilder.DropIndex(
                name: "IX_DietPlanAssignments_MemberId_IsActive",
                table: "DietPlanAssignments");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "DietPlanAssignments");

            migrationBuilder.RenameColumn(
                name: "WorkoutPlanId",
                table: "Memberships",
                newName: "WorkoutAssignmentId");

            migrationBuilder.RenameColumn(
                name: "DietPlanId1",
                table: "Memberships",
                newName: "DietPlanAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Memberships_WorkoutPlanId",
                table: "Memberships",
                newName: "IX_Memberships_WorkoutAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Memberships_DietPlanId1",
                table: "Memberships",
                newName: "IX_Memberships_DietPlanAssignmentId");

            migrationBuilder.AlterColumn<string>(
                name: "MemberId",
                table: "WorkoutAssignments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_DietPlanAssignments_DietPlanAssignmentId",
                table: "Memberships",
                column: "DietPlanAssignmentId",
                principalTable: "DietPlanAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_DietPlans_DietPlanId",
                table: "Memberships",
                column: "DietPlanId",
                principalTable: "DietPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_WorkoutAssignments_WorkoutAssignmentId",
                table: "Memberships",
                column: "WorkoutAssignmentId",
                principalTable: "WorkoutAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutAssignments_Members_MemberId",
                table: "WorkoutAssignments",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_DietPlanAssignments_DietPlanAssignmentId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_DietPlans_DietPlanId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_WorkoutAssignments_WorkoutAssignmentId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutAssignments_Members_MemberId",
                table: "WorkoutAssignments");

            migrationBuilder.RenameColumn(
                name: "WorkoutAssignmentId",
                table: "Memberships",
                newName: "WorkoutPlanId");

            migrationBuilder.RenameColumn(
                name: "DietPlanAssignmentId",
                table: "Memberships",
                newName: "DietPlanId1");

            migrationBuilder.RenameIndex(
                name: "IX_Memberships_WorkoutAssignmentId",
                table: "Memberships",
                newName: "IX_Memberships_WorkoutPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_Memberships_DietPlanAssignmentId",
                table: "Memberships",
                newName: "IX_Memberships_DietPlanId1");

            migrationBuilder.AlterColumn<string>(
                name: "MemberId",
                table: "WorkoutAssignments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberId",
                table: "DietPlanAssignments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutAssignments_MemberId_IsActive",
                table: "WorkoutAssignments",
                columns: new[] { "MemberId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DietPlanAssignments_MemberId",
                table: "DietPlanAssignments",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DietPlanAssignments_MemberId_IsActive",
                table: "DietPlanAssignments",
                columns: new[] { "MemberId", "IsActive" });

            migrationBuilder.AddForeignKey(
                name: "FK_DietPlanAssignments_Members_MemberId",
                table: "DietPlanAssignments",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_DietPlans_DietPlanId",
                table: "Memberships",
                column: "DietPlanId",
                principalTable: "DietPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_DietPlans_DietPlanId1",
                table: "Memberships",
                column: "DietPlanId1",
                principalTable: "DietPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_WorkoutPlans_WorkoutPlanId",
                table: "Memberships",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutAssignments_Members_MemberId",
                table: "WorkoutAssignments",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
