using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDAL.Migrations
{
    /// <inheritdoc />
    public partial class fixreviewjoindatemissing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainerReviews_AspNetUsers_MemberId",
                table: "TrainerReviews");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerReviews_Members_MemberId",
                table: "TrainerReviews",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainerReviews_Members_MemberId",
                table: "TrainerReviews");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerReviews_AspNetUsers_MemberId",
                table: "TrainerReviews",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
