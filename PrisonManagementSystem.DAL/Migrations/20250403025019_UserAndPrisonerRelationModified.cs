using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrisonManagementSystem.DAL.Migrations
{
    public partial class UserAndPrisonerRelationModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prisoners_AspNetUsers_UserId",
                table: "Prisoners");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Prisoners",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Prisoners_AspNetUsers_UserId",
                table: "Prisoners",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prisoners_AspNetUsers_UserId",
                table: "Prisoners");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Prisoners",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Prisoners_AspNetUsers_UserId",
                table: "Prisoners",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
