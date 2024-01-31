using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleHelper.Infrastructure.Migrations
{
    public partial class AddStartTimeForSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ScheduleSettings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "ScheduleSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ScheduleSettings");

            migrationBuilder.InsertData(
                table: "ScheduleSettings",
                columns: new[] { "Id", "FinishTime", "breakDurationMin" },
                values: new object[] { 1, new DateTime(1, 1, 1, 1, 1, 0, 0, DateTimeKind.Unspecified), 21.0 });
        }
    }
}
