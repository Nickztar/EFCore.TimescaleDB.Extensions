using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApp.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forecasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TemperatureC = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.Sql(
                "SELECT create_hypertable( '\"Forecasts\"', by_range('Date', INTERVAL '1 day'));"
            );
            migrationBuilder.Sql(
                "SELECT add_retention_policy( '\"Forecasts\"', INTERVAL '8 days');"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Forecasts");

            migrationBuilder.Sql(
                """
DO
$do$
    BEGIN
        IF (select to_regclass('"Forecasts"') is not null) THEN
            PERFORM remove_retention_policy('"Forecasts"', if_exists := true);
        ELSE
            PERFORM 'NOOP' as Noop;
        END IF;
    END
$do$
"""
            );
        }
    }
}
