using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExtremeInsiders.Migrations
{
    public partial class AddedDuration_ToMovieAndVideo2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "EntitiesBase");

            migrationBuilder.DropColumn(
                name: "Video_Duration",
                table: "EntitiesBase");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "VideosTranslations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "MoviesTranslations",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 20, 59, 34, 638, DateTimeKind.Utc).AddTicks(9483));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 20, 59, 34, 638, DateTimeKind.Utc).AddTicks(8616));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 20, 59, 34, 638, DateTimeKind.Utc).AddTicks(8043));

            migrationBuilder.UpdateData(
                table: "EntitiesBase",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2021, 5, 5, 20, 59, 34, 638, DateTimeKind.Utc).AddTicks(9230));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "VideosTranslations");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "MoviesTranslations");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "EntitiesBase",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video_Duration",
                table: "EntitiesBase",
                type: "text",
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
    }
}
