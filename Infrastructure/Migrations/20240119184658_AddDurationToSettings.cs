using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleHelper.Infrastructure.Migrations
{
    public partial class AddDurationToSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "breakDurationMin",
                table: "ScheduleSettings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "ScheduleSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "breakDurationMin",
                value: 21.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "breakDurationMin",
                table: "ScheduleSettings");
        }
    }
}
