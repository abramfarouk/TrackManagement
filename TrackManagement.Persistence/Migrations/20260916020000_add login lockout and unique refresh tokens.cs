using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackManagement.Persistence.Migrations;

public partial class addloginlockoutanduniquerefreshtokens : Migration
{
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
            DELETE FROM RefreshTokens
            WHERE Id IN (SELECT Id FROM RankedTokens WHERE RowNumber > 1);");

        migrationBuilder.CreateIndex(
            name: "IX_RefreshTokens_Username",
            table: "RefreshTokens",
            column: "Username",
            unique: true);
    }

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
