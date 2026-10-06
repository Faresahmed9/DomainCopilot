using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DomainCopilot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HashUserPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
    UPDATE Users
    SET Password = 'AQAAAAEAAYagAAAAEI3WjmCjyC5ssbiOhGUj2s6Tf3EfK2MJzLmcs1EkEI22n2bmkfYwzo1lgiLlhESKaQ=='
    WHERE UserId = 'A1111111-1111-1111-1111-111111111111';

    UPDATE Users
    SET Password = 'AQAAAAEAAYagAAAAEKCEFfCaQu8h6HC1/I5w4trHj/m4PZDYCTSMN7Pr2bVWlEQbEqjhSr9BQdc/Pt7s6Q=='
    WHERE UserId = 'A2222222-2222-2222-2222-222222222222';

    UPDATE Users
    SET Password = 'AQAAAAEAAYagAAAAEI3WjmCjyC5ssbiOhGUj2s6Tf3EfK2MJzLmcs1EkEI22n2bmkfYwzo1lgiLlhESKaQ=='
    WHERE UserId = 'B1111111-1111-1111-1111-111111111111';

    UPDATE Users
    SET Password = 'AQAAAAEAAYagAAAAEKCEFfCaQu8h6HC1/I5w4trHj/m4PZDYCTSMN7Pr2bVWlEQbEqjhSr9BQdc/Pt7s6Q=='
    WHERE UserId = 'B2222222-2222-2222-2222-222222222222';
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        UPDATE Users
        SET Password = 'Admin123!'
        WHERE UserId = 'A1111111-1111-1111-1111-111111111111';

        UPDATE Users
        SET Password = 'Adjuster123!'
        WHERE UserId = 'A2222222-2222-2222-2222-222222222222';

        UPDATE Users
        SET Password = 'Admin123!'
        WHERE UserId = 'B1111111-1111-1111-1111-111111111111';

        UPDATE Users
        SET Password = 'Adjuster123!'
        WHERE UserId = 'B2222222-2222-2222-2222-222222222222';
    """);
        }
    }
}
