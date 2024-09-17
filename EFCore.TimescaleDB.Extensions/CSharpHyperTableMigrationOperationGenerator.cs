using EFCore.TimescaleDB.Extensions.Operations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EFCore.TimescaleDB.Extensions
{
    public class CSharpHyperTableMigrationOperationGenerator(CSharpMigrationOperationGeneratorDependencies dependencies)
        : CSharpMigrationOperationGenerator(dependencies)
    {
        //Debugger.Launch();

        protected override void Generate(MigrationOperation operation, IndentedStringBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(operation);
            ArgumentNullException.ThrowIfNull(builder);

            switch (operation)
            {
                case CreateHyperTableOperation create:
                    Generate(create, builder);
                    break;
                    
                case ChangeHyperTableOperation change:
                    Generate(change, builder);
                    break;

                case DropHyperRetentionOperation drop:
                    Generate(drop, builder);
                    break;
                    
                default:
                    base.Generate(operation, builder);
                    break;
            }
        }

        private static void Generate(CreateHyperTableOperation operation, IndentedStringBuilder builder)
        {
            var interval = operation.ChunkSize is null ? "" : $", INTERVAL '{operation.ChunkSize}'";
            var hyperTableStatement = $"SELECT create_hypertable( '\\\"{operation.TableName}\\\"', by_range('{operation.HyperProperty.Name}'{interval}));";
            builder.CreateSqlStatement(
                hyperTableStatement,
                isFinalStatement: operation.Retention is null
            );
            if (operation.Retention is not null)
            {
                builder.CreateSqlStatement(
                    $"SELECT add_retention_policy( '\\\"{operation.TableName}\\\"', INTERVAL '{operation.Retention}');", 
                    withBuilder: true, 
                    isFinalStatement: true
                );
            }
        }
        
        private static void Generate(ChangeHyperTableOperation operation, IndentedStringBuilder builder)
        {
            if (operation.Retention != null)
            {
                builder.CreateSqlStatement(
                    $"SELECT add_retention_policy( '\\\"{operation.TableName}\\\"', INTERVAL '{operation.Retention}');", 
                    isFinalStatement: operation.ChunkSize is null
                );
            }
            if (operation.ChunkSize != null)
            {
                builder.CreateSqlStatement(
                    $"SELECT set_chunk_time_interval('\\\"{operation.TableName}\\\"', INTERVAL '{operation.ChunkSize}');", 
                    withBuilder: operation.Retention != null,
                    isFinalStatement: true
                );
            }
        }

        private static void Generate(DropHyperRetentionOperation operation, IndentedStringBuilder builder)
        {
            builder.CreateSqlStatement(
                $"""
                DO
                $do$
                    BEGIN
                        IF (select to_regclass('"{operation.Name}"') is not null) THEN
                            PERFORM remove_retention_policy('"{operation.Name}"', if_exists := true);
                        ELSE
                            PERFORM 'NOOP' as Noop;
                        END IF;
                    END
                $do$
                """,  
                tripleQuote: true,
                isFinalStatement: true);
        }
    }
}
