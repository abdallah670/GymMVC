using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDAL.Migrations
{
    /// <inheritdoc />
    public partial class addreportandreview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutAssignments_Members_MemberId",
                table: "WorkoutAssignments");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutAssignments_MemberId",
                table: "WorkoutAssignments");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "WorkoutAssignments");

            migrationBuilder.AddColumn<string>(
                name: "ActivityLevel",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrainerReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainerReviews_AspNetUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainerReviews_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkoutPlanId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutLogs_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutLogs_WorkoutPlans_WorkoutPlanId",
                        column: x => x.WorkoutPlanId,
                        principalTable: "WorkoutPlans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkoutLogEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkoutLogId = table.Column<int>(type: "int", nullable: false),
                    WorkoutPlanItemId = table.Column<int>(type: "int", nullable: true),
                    ExerciseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SetsPerformed = table.Column<int>(type: "int", nullable: false),
                    RepsPerformed = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WeightLifted = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutLogEntries_WorkoutLogs_WorkoutLogId",
                        column: x => x.WorkoutLogId,
                        principalTable: "WorkoutLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutLogEntries_WorkoutPlanItems_WorkoutPlanItemId",
                        column: x => x.WorkoutPlanItemId,
                        principalTable: "WorkoutPlanItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerReviews_MemberId",
                table: "TrainerReviews",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerReviews_TrainerId",
                table: "TrainerReviews",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLogEntries_WorkoutLogId",
                table: "WorkoutLogEntries",
                column: "WorkoutLogId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLogEntries_WorkoutPlanItemId",
                table: "WorkoutLogEntries",
                column: "WorkoutPlanItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLogs_MemberId",
                table: "WorkoutLogs",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLogs_WorkoutPlanId",
                table: "WorkoutLogs",
                column: "WorkoutPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainerReviews");

            migrationBuilder.DropTable(
                name: "WorkoutLogEntries");

            migrationBuilder.DropTable(
                name: "WorkoutLogs");

            migrationBuilder.DropColumn(
                name: "ActivityLevel",
                table: "Members");

            migrationBuilder.AddColumn<string>(
                name: "MemberId",
                table: "WorkoutAssignments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutAssignments_MemberId",
                table: "WorkoutAssignments",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutAssignments_Members_MemberId",
                table: "WorkoutAssignments",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }
    }
}
