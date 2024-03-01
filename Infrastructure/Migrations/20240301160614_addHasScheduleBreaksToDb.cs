using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleHelper.Infrastructure.Migrations
{
    public partial class addHasScheduleBreaksToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DaySchedule_ScheduleSettings_settingsId",
                table: "DaySchedule");

            migrationBuilder.RenameColumn(
                name: "timeFromLastBreakMin",
                table: "DaySchedule",
                newName: "TimeFromLastBreakMin");

            migrationBuilder.RenameColumn(
                name: "settingsId",
                table: "DaySchedule",
                newName: "SettingsId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "DaySchedule",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_DaySchedule_settingsId",
                table: "DaySchedule",
                newName: "IX_DaySchedule_SettingsId");

            migrationBuilder.AddColumn<bool>(
                name: "HasScheduleBreaks",
                table: "ScheduleSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_DaySchedule_ScheduleSettings_SettingsId",
                table: "DaySchedule",
                column: "SettingsId",
                principalTable: "ScheduleSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DaySchedule_ScheduleSettings_SettingsId",
                table: "DaySchedule");

            migrationBuilder.DropColumn(
                name: "HasScheduleBreaks",
                table: "ScheduleSettings");

            migrationBuilder.RenameColumn(
                name: "TimeFromLastBreakMin",
                table: "DaySchedule",
                newName: "timeFromLastBreakMin");

            migrationBuilder.RenameColumn(
                name: "SettingsId",
                table: "DaySchedule",
                newName: "settingsId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DaySchedule",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_DaySchedule_SettingsId",
                table: "DaySchedule",
                newName: "IX_DaySchedule_settingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_DaySchedule_ScheduleSettings_settingsId",
                table: "DaySchedule",
                column: "settingsId",
                principalTable: "ScheduleSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
