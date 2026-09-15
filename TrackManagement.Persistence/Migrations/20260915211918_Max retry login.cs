using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Maxretrylogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedUntilUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"
                WITH RankedTokens AS (
                    SELECT Id,
                           ROW_NUMBER() OVER (PARTITION BY Username ORDER BY Id DESC) AS RowNumber
                    FROM RefreshTokens
                )
                DELETE FROM RankedTokens
                WHERE RowNumber > 1;");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Username",
                table: "RefreshTokens",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_Username",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LockedUntilUtc",
                table: "Users");
        }
    }
}
