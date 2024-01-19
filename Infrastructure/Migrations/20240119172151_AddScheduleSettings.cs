using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleHelper.Infrastructure.Migrations
{
    public partial class AddScheduleSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TimeSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ScheduleSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinishTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ScheduleSettings",
                columns: new[] { "Id", "FinishTime" },
                values: new object[] { 1, new DateTime(1, 1, 1, 1, 1, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleSettings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TimeSlots");
        }
    }
}
