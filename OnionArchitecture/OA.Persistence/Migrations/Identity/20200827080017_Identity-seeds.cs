using Microsoft.EntityFrameworkCore.Migrations;

namespace OA.Persistence.Migrations.Identity
{
    public partial class Identityseeds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Role",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "db13a068-a146-4611-891f-934d985014b7", "bd14543f-c0cd-45bd-884f-163c7504296b", "SuperAdmin", "SuperAdmin" },
                    { "6d7e3228-8f84-4fa9-b6bc-3a7c007171fb", "981f6cda-ea58-4a9b-a7cc-8fb5abd3d176", "Admin", "Admin" },
                    { "468ae608-1cdf-4ce2-8e1b-80ffcf96e691", "031dfeeb-e5e3-417f-a451-39ca91f394a5", "Moderator", "Moderator" },
                    { "b349cfba-6266-43e2-b5bc-24903ad36eae", "ba4ffbed-f780-4161-a519-45089a2385e6", "Basic", "Basic" }
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "User",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "9820b9bd-ce1d-4980-bef6-5bc4df6f1c14", 0, "a3e0c9da-85e0-4d24-a947-c6b6984f662b", "superadmin@gmail.com", true, "Amit", "Naik", false, null, null, null, "AQAAAAEAACcQAAAAEDHLzgfZnYTrv5i9YLqajV90Wgdmf7xECY3nUbOk4r8YM3m/nCPZe2sJ4UW7WWBfBg==", null, true, "86c90357-782a-4de6-811c-7017630715bc", false, "superadmin" },
                    { "cdb76fda-e86a-4558-a597-435fc9e46bb6", 0, "0f4a71a1-e63c-4f65-8417-cf6abd12b2df", "basicuser@gmail.com", true, "Basic", "User", false, null, null, null, "AQAAAAEAACcQAAAAEGsSwGdcvv0LGkk8yYKwJYsK6XUaiFh8sv33mJT1SeKNW1Fd1oF4GJvrixdK5QpJjQ==", null, true, "7eb836e6-a9f2-4b2d-b08b-e3fd10f78cad", false, "basicuser" }
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { "9820b9bd-ce1d-4980-bef6-5bc4df6f1c14", "db13a068-a146-4611-891f-934d985014b7" },
                    { "9820b9bd-ce1d-4980-bef6-5bc4df6f1c14", "6d7e3228-8f84-4fa9-b6bc-3a7c007171fb" },
                    { "9820b9bd-ce1d-4980-bef6-5bc4df6f1c14", "468ae608-1cdf-4ce2-8e1b-80ffcf96e691" },
                    { "9820b9bd-ce1d-4980-bef6-5bc4df6f1c14", "b349cfba-6266-43e2-b5bc-24903ad36eae" },
                    { "cdb76fda-e86a-4558-a597-435fc9e46bb6", "b349cfba-6266-43e2-b5bc-24903ad36eae" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "UserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "9820b9bd-ce1d-4980-bef6-5bc4df6f1c14", "468ae608-1cdf-4ce2-8e1b-80ffcf96e691" });

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "UserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "9820b9bd-ce1d-4980-bef6-5bc4df6f1c14", "6d7e3228-8f84-4fa9-b6bc-3a7c007171fb" });

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "UserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "9820b9bd-ce1d-4980-bef6-5bc4df6f1c14", "b349cfba-6266-43e2-b5bc-24903ad36eae" });

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "UserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "9820b9bd-ce1d-4980-bef6-5bc4df6f1c14", "db13a068-a146-4611-891f-934d985014b7" });

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "UserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "cdb76fda-e86a-4558-a597-435fc9e46bb6", "b349cfba-6266-43e2-b5bc-24903ad36eae" });

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Role",
                keyColumn: "Id",
                keyValue: "468ae608-1cdf-4ce2-8e1b-80ffcf96e691");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Role",
                keyColumn: "Id",
                keyValue: "6d7e3228-8f84-4fa9-b6bc-3a7c007171fb");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Role",
                keyColumn: "Id",
                keyValue: "b349cfba-6266-43e2-b5bc-24903ad36eae");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Role",
                keyColumn: "Id",
                keyValue: "db13a068-a146-4611-891f-934d985014b7");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "User",
                keyColumn: "Id",
                keyValue: "9820b9bd-ce1d-4980-bef6-5bc4df6f1c14");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "User",
                keyColumn: "Id",
                keyValue: "cdb76fda-e86a-4558-a597-435fc9e46bb6");
        }
    }
}
