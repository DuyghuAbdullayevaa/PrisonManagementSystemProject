using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrisonManagementSystem.DAL.Migrations
{
    public partial class AddPositionTypeEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Staffs");
        }
    }
}
