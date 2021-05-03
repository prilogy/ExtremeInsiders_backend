using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExtremeInsiders.Migrations
{
    public partial class AddedInAppPurchaseKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppleInAppPurchaseKey",
                table: "SubscriptionsPlans",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleInAppPurchaseKey",
                table: "SubscriptionsPlans",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppleInAppPurchaseKey",
                table: "SubscriptionsPlans");

            migrationBuilder.DropColumn(
                name: "GoogleInAppPurchaseKey",
                table: "SubscriptionsPlans");

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2021, 5, 3, 10, 56, 24, 848, DateTimeKind.Utc).AddTicks(758));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2021, 5, 3, 10, 56, 24, 848, DateTimeKind.Utc).AddTicks(27));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 5, 3, 10, 56, 24, 847, DateTimeKind.Utc).AddTicks(9428));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2021, 5, 3, 10, 56, 24, 848, DateTimeKind.Utc).AddTicks(517));
        }
    }
}
