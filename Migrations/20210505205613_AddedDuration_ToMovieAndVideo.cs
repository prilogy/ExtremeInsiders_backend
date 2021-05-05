using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExtremeInsiders.Migrations
{
    public partial class AddedDuration_ToMovieAndVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "EntitiesBase",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video_Duration",
                table: "EntitiesBase",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 20, 56, 13, 248, DateTimeKind.Utc).AddTicks(2220));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 20, 56, 13, 248, DateTimeKind.Utc).AddTicks(1398));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 20, 56, 13, 248, DateTimeKind.Utc).AddTicks(823));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 20, 56, 13, 248, DateTimeKind.Utc).AddTicks(1960));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "EntitiesBase");

            migrationBuilder.DropColumn(
                name: "Video_Duration",
                table: "EntitiesBase");

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
        }
    }
}
