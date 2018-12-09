using Microsoft.EntityFrameworkCore.Migrations;

namespace RetroDb.DataSqlite.Migrations
{
    public partial class AddTimesPlayed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimesPlayed",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimesPlayed",
                table: "Games");
        }
    }
}
