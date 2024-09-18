using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApp.Migrations
{
    /// <inheritdoc />
    public partial class Inhertied : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inherited",
                columns: table => new
                {
                    Text = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.Sql(
                "SELECT create_hypertable( '\"Inherited\"', by_range('Time'));"
            );
            migrationBuilder.Sql(
                "SELECT add_retention_policy( '\"Inherited\"', INTERVAL '1 day');"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inherited");

            migrationBuilder.Sql(
                """
DO
$do$
    BEGIN
        IF (select to_regclass('"Inherited"') is not null) THEN
            PERFORM remove_retention_policy('"Inherited"', if_exists := true);
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
