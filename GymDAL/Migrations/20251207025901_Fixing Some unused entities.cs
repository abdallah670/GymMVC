using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDAL.Migrations
{
    /// <inheritdoc />
    public partial class FixingSomeunusedentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanAssignments_Members_MemberId1",
                table: "DietPlanAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_FitnessGoals_Members_MemberId",
                table: "FitnessGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_FitnessGoals_Subscription_SubscriptionId",
                table: "FitnessGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Members_MemberId",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Memberships_MembershipId",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Payments_PaymentId",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_FitnessGoals_MemberId",
                table: "FitnessGoals");

            migrationBuilder.DropIndex(
                name: "IX_FitnessGoals_SubscriptionId",
                table: "FitnessGoals");

            migrationBuilder.DropIndex(
                name: "IX_DietPlanAssignments_MemberId1",
                table: "DietPlanAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription");

       

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "FitnessGoals");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "FitnessGoals");

            migrationBuilder.DropColumn(
                name: "MemberId1",
                table: "DietPlanAssignments");

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

            migrationBuilder.AddColumn<int>(
                name: "FitnessGoalId",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MemberId",
                table: "DietPlanAssignments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Members_FitnessGoalId",
                table: "Members",
                column: "FitnessGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_DietPlanAssignments_Members_MemberId",
                table: "DietPlanAssignments",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_FitnessGoals_FitnessGoalId",
                table: "Members",
                column: "FitnessGoalId",
                principalTable: "FitnessGoals",
                principalColumn: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanAssignments_Members_MemberId",
                table: "DietPlanAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_FitnessGoals_FitnessGoalId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Members_MemberId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Payments_PaymentId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Members_FitnessGoalId",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "FitnessGoalId",
                table: "Members");

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

            migrationBuilder.AddColumn<double>(
                name: "TargetWeight",
                table: "Members",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "MemberId",
                table: "FitnessGoals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "FitnessGoals",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "DietPlanAssignments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "MemberId1",
                table: "DietPlanAssignments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Subscription",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FitnessGoals_MemberId",
                table: "FitnessGoals",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_FitnessGoals_SubscriptionId",
                table: "FitnessGoals",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_DietPlanAssignments_MemberId1",
                table: "DietPlanAssignments",
                column: "MemberId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DietPlanAssignments_Members_MemberId1",
                table: "DietPlanAssignments",
                column: "MemberId1",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FitnessGoals_Members_MemberId",
                table: "FitnessGoals",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FitnessGoals_Subscription_SubscriptionId",
                table: "FitnessGoals",
                column: "SubscriptionId",
                principalTable: "Subscription",
                principalColumn: "Id");

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
    }
}
