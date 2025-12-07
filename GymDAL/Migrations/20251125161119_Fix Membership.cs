using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDAL.Migrations
{
    /// <inheritdoc />
    public partial class FixMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasWorkoutPlan",
                table: "Memberships",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasWorkoutPlan",
                table: "Memberships");
        }
    }
}
