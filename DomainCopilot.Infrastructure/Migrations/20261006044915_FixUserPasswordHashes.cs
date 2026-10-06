using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainCopilot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserPasswordHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAELsJ6ksJI8Ya9B5alBr/o2HFqsyheAnvLL7WMs9Z82deO3zpQ54h6/mUv9e9mbA35Q==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222222"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHcusyGKVyYXrprjsLRPoeOMa/9+/8LJrH6I9QWPR2bTc+YY1x7ZHqIhE8STc2ma1g==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111111"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEAlp3qr3v3T35+pIUjeLIvhleN1Ai2oGcqoCie4CmPVbvfhJU5igvfba9t/QG1SDww==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHX7i4q55MfTE9ZHdjJcVXfcSvE7FTYTuzRSFudZfRJ9H2LvQIusBbp5XwEFBn6L1g==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"),
                column: "Password",
                value: "AQAAAAEAAYagAAAAEI3WjmCjyC5ssbiOhGUj2s6Tf3EfK2MJzLmcs1EkEI22n2bmkfYwzo1lgiLlhESKaQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222222"),
                column: "Password",
                value: "AQAAAAEAAYagAAAAEKCEFfCaQu8h6HC1/I5w4trHj/m4PZDYCTSMN7Pr2bVWlEQbEqjhSr9BQdc/Pt7s6Q==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111111"),
                column: "Password",
                value: "AQAAAAEAAYagAAAAEI3WjmCjyC5ssbiOhGUj2s6Tf3EfK2MJzLmcs1EkEI22n2bmkfYwzo1lgiLlhESKaQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                column: "Password",
                value: "AQAAAAEAAYagAAAAEKCEFfCaQu8h6HC1/I5w4trHj/m4PZDYCTSMN7Pr2bVWlEQbEqjhSr9BQdc/Pt7s6Q==");
        }
    }
}
