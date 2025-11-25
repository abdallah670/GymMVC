using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameEntites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Members_MemberId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Payments_PaymentId",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.RenameTable(
                name: "Subscriptions",
                newName: "Subscription");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_PaymentId",
                table: "Subscription",
                newName: "IX_Subscription_PaymentId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_MembershipId",
                table: "Subscription",
                newName: "IX_Subscription_MembershipId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_MemberId",
                table: "Subscription",
                newName: "IX_Subscription_MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Members_MemberId",
                table: "Subscription",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Memberships_MembershipId",
                table: "Subscription",
                column: "MembershipId",
                principalTable: "Memberships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Payments_PaymentId",
                table: "Subscription",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Members_MemberId",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Memberships_MembershipId",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Payments_PaymentId",
                table: "Subscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription");

            migrationBuilder.RenameTable(
                name: "Subscription",
                newName: "Subscriptions");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_PaymentId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_PaymentId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_MembershipId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_MembershipId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_MemberId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Members_MemberId",
                table: "Subscriptions",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipId",
                table: "Subscriptions",
                column: "MembershipId",
                principalTable: "Memberships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Payments_PaymentId",
                table: "Subscriptions",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
