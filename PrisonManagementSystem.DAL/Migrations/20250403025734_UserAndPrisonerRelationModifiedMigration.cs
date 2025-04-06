using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrisonManagementSystem.DAL.Migrations
{
    public partial class UserAndPrisonerRelationModifiedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prisoners_AspNetUsers_UserId",
                table: "Prisoners");

            migrationBuilder.DropIndex(
                name: "IX_Prisoners_UserId",
                table: "Prisoners");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Prisoners");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Prisoners",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prisoners_UserId",
                table: "Prisoners",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prisoners_AspNetUsers_UserId",
                table: "Prisoners",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
