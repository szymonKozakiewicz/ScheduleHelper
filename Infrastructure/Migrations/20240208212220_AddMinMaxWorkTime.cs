using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleHelper.Infrastructure.Migrations
{
    public partial class AddMinMaxWorkTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MaxWorkTimeBeforeBreakMin",
                table: "ScheduleSettings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MinWorkTimeBeforeBreakMin",
                table: "ScheduleSettings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxWorkTimeBeforeBreakMin",
                table: "ScheduleSettings");

            migrationBuilder.DropColumn(
                name: "MinWorkTimeBeforeBreakMin",
                table: "ScheduleSettings");
        }
    }
}
