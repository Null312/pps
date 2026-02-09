using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PaymentService.Migrations
{
    /// <inheritdoc />
    public partial class AddedFaceUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName" },
                values: new object[,]
                {
                    { new Guid("a6a7ef54-b3d2-4fc8-bc84-004d65a168d6"), new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "test2@test.com", null },
                    { new Guid("a6a7ef54-b3d2-4fc8-bc84-004d65a168d8"), new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "test1@test.com", null }
                });

            migrationBuilder.InsertData(
                table: "accounts",
                columns: new[] { "Id", "AccountNumber", "Balance", "CreatedAt", "Currency", "DailyLimit", "IsBlocked", "UserId" },
                values: new object[,]
                {
                    { new Guid("c765d7d5-e62c-4ac6-859d-85c9a803cb41"), "a2", 1000m, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "USD", 5000m, false, new Guid("a6a7ef54-b3d2-4fc8-bc84-004d65a168d6") },
                    { new Guid("c765d7d5-e62c-4ac6-859d-85c9a803cb42"), "a1", 10000m, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "USD", 5000m, false, new Guid("a6a7ef54-b3d2-4fc8-bc84-004d65a168d8") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accounts",
                keyColumn: "Id",
                keyValue: new Guid("c765d7d5-e62c-4ac6-859d-85c9a803cb41"));

            migrationBuilder.DeleteData(
                table: "accounts",
                keyColumn: "Id",
                keyValue: new Guid("c765d7d5-e62c-4ac6-859d-85c9a803cb42"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("a6a7ef54-b3d2-4fc8-bc84-004d65a168d6"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("a6a7ef54-b3d2-4fc8-bc84-004d65a168d8"));
        }
    }
}
