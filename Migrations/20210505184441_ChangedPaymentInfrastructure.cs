using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExtremeInsiders.Migrations
{
    public partial class ChangedPaymentInfrastructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Sales",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProviderType",
                table: "Payments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AppleInAppPurchaseKey",
                table: "EntitiesBase",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleInAppPurchaseKey",
                table: "EntitiesBase",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 18, 44, 40, 811, DateTimeKind.Utc).AddTicks(485));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 18, 44, 40, 810, DateTimeKind.Utc).AddTicks(9764));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 18, 44, 40, 810, DateTimeKind.Utc).AddTicks(9217));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 18, 44, 40, 811, DateTimeKind.Utc).AddTicks(248));

            migrationBuilder.InsertData(
                table: "SocialAccountsProviders",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "apple" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PaymentId",
                table: "Sales",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Payments_PaymentId",
                table: "Sales",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Payments_PaymentId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PaymentId",
                table: "Sales");

            migrationBuilder.DeleteData(
                table: "SocialAccountsProviders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ProviderType",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AppleInAppPurchaseKey",
                table: "EntitiesBase");

            migrationBuilder.DropColumn(
                name: "GoogleInAppPurchaseKey",
                table: "EntitiesBase");

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2021, 5, 3, 10, 57, 0, 960, DateTimeKind.Utc).AddTicks(5200));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2021, 5, 3, 10, 57, 0, 960, DateTimeKind.Utc).AddTicks(4458));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 5, 3, 10, 57, 0, 960, DateTimeKind.Utc).AddTicks(3905));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2021, 5, 3, 10, 57, 0, 960, DateTimeKind.Utc).AddTicks(4954));
        }
    }
}
