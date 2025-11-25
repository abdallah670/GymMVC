using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDAL.Migrations
{
    /// <inheritdoc />
    public partial class Refactormember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentWeight",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "TargetWeight",
                table: "Subscription");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "CurrentWeight",
                table: "Members",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "HasCompletedProfile",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Height",
                table: "Members",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TargetWeight",
                table: "Members",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CurrentWeight",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "HasCompletedProfile",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "TargetWeight",
                table: "Members");

            migrationBuilder.AddColumn<double>(
                name: "CurrentWeight",
                table: "Subscription",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Height",
                table: "Subscription",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TargetWeight",
                table: "Subscription",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
