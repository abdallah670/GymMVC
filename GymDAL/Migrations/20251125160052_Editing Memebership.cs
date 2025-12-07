using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDAL.Migrations
{
    /// <inheritdoc />
    public partial class EditingMemebership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Members_MemberId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_EndDate",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_MemberId_Status",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_StartDate",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_TrainerId_Status",
                table: "Memberships");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "WorkoutPlans",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "WorkoutPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Beginner");

            migrationBuilder.AddColumn<string>(
                name: "Goal",
                table: "WorkoutPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "General Fitness");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WorkoutPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Memberships",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Memberships",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Memberships",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "DietPlanId",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DietPlanId1",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkoutPlanId",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkoutPlanId1",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "DietPlans",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DietType",
                table: "DietPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Balanced");

            migrationBuilder.AddColumn<int>(
                name: "DurationDays",
                table: "DietPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "DietPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_DietPlanId",
                table: "Memberships",
                column: "DietPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_DietPlanId1",
                table: "Memberships",
                column: "DietPlanId1");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_StartDate_EndDate",
                table: "Memberships",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_WorkoutPlanId",
                table: "Memberships",
                column: "WorkoutPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_WorkoutPlanId1",
                table: "Memberships",
                column: "WorkoutPlanId1");

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
                name: "FK_Memberships_Members_MemberId",
                table: "Memberships",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_WorkoutPlans_WorkoutPlanId",
                table: "Memberships",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_WorkoutPlans_WorkoutPlanId1",
                table: "Memberships",
                column: "WorkoutPlanId1",
                principalTable: "WorkoutPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_DietPlans_DietPlanId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_DietPlans_DietPlanId1",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Members_MemberId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_WorkoutPlans_WorkoutPlanId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_WorkoutPlans_WorkoutPlanId1",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_DietPlanId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_DietPlanId1",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_StartDate_EndDate",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_WorkoutPlanId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_WorkoutPlanId1",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "WorkoutPlans");

            migrationBuilder.DropColumn(
                name: "Goal",
                table: "WorkoutPlans");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WorkoutPlans");

            migrationBuilder.DropColumn(
                name: "DietPlanId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "DietPlanId1",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "WorkoutPlanId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "WorkoutPlanId1",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "DietType",
                table: "DietPlans");

            migrationBuilder.DropColumn(
                name: "DurationDays",
                table: "DietPlans");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "DietPlans");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "WorkoutPlans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Memberships",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Memberships",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Memberships",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "DietPlans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_EndDate",
                table: "Memberships",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_MemberId_Status",
                table: "Memberships",
                columns: new[] { "MemberId", "Status" },
                filter: "[Status] = 'Active'");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_StartDate",
                table: "Memberships",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_TrainerId_Status",
                table: "Memberships",
                columns: new[] { "TrainerId", "Status" },
                filter: "[Status] = 'Active'");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Members_MemberId",
                table: "Memberships",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
