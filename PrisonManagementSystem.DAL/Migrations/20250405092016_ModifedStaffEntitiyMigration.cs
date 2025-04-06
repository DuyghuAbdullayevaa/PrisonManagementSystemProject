using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrisonManagementSystem.DAL.Migrations
{
    public partial class ModifedStaffEntitiyMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Staffs_StaffId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_StaffId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Reports");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b1a7c630-88e0-4fbd-a8a1-0123456789ab",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ddf10123-5018-4cc1-9561-8fd012a14928", "AQAAAAEAACcQAAAAEGe973JCXZgng4PaD8S0iVKgUcWjaIzb/BfJerlj6g28aVs6y5FbkubX359jkpy6zw==", "1d56f1a7-ebf9-428e-9a83-5678266679f5" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StaffId",
                table: "Reports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b1a7c630-88e0-4fbd-a8a1-0123456789ab",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f42f67ed-6ac2-4183-ad52-05ddbb29dbb9", "AQAAAAEAACcQAAAAEF/J+cyuHsbfOtbLKo4aXGeSgSmlhpHJUQeI6nPGxNYSHkwJmC7/EnejdDoWtiFCUA==", "09310d18-14f0-452e-972c-ec4f828c3c8c" });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_StaffId",
                table: "Reports",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Staffs_StaffId",
                table: "Reports",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id");
        }
    }
}
