using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrisonManagementSystem.DAL.Migrations
{
    public partial class ModifedAdminPhoneNUmberMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "688aa13f-29b8-4f2e-8ac8-75a9c3fa25b9");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "BirthDate", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b1a7c630-88e0-4fbd-a8a1-0123456789ab", 0, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "f42f67ed-6ac2-4183-ad52-05ddbb29dbb9", "admin@prison.com", false, "Admin", "User", false, null, "ADMIN@PRISON.COM", "ADMIN@PRISON.COM", "AQAAAAEAACcQAAAAEF/J+cyuHsbfOtbLKo4aXGeSgSmlhpHJUQeI6nPGxNYSHkwJmC7/EnejdDoWtiFCUA==", "+994501234567", false, "09310d18-14f0-452e-972c-ec4f828c3c8c", false, "admin@prison.com" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b1a7c630-88e0-4fbd-a8a1-0123456789ab");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "BirthDate", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "688aa13f-29b8-4f2e-8ac8-75a9c3fa25b9", 0, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "3c64d69c-4e10-4ad4-8413-863d4519bde5", "admin@prison.com", false, "Admin", "User", false, null, "ADMIN@PRISON.COM", "ADMIN@PRISON.COM", "AQAAAAEAACcQAAAAEE9H2chZIjQUEdLe/owGC+NYZ449Ltw3SRj7WUG5rg6DQ2I3SBTXPE+irNcBkqQ0gg==", null, false, "dfc8b2b7-483f-4efd-9e56-01cdbeca8cda", false, "admin@prison.com" });
        }
    }
}
