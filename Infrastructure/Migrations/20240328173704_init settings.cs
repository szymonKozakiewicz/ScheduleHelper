using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleHelper.Infrastructure.Migrations
{
    public partial class initsettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ScheduleSettings",
                columns: new[] { "Id", "FinishTime", "HasScheduleBreaks", "MaxWorkTimeBeforeBreakMin", "MinWorkTimeBeforeBreakMin", "StartTime", "breakDurationMin" },
                values: new object[] { 1, new DateTime(1, 1, 1, 21, 0, 0, 0, DateTimeKind.Unspecified), false, 60.0, 45.0, new DateTime(1, 1, 1, 10, 9, 0, 0, DateTimeKind.Unspecified), 20.0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ScheduleSettings",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
