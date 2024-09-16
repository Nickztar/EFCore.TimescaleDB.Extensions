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
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            switch (operation)
            {
                case CreateHyperTableOperation create:
                    Generate(create, builder);
                    break;

                case DropHyperTableOperation drop:
                    Generate(drop, builder);
                    break;

                default:
                    base.Generate(operation, builder);
                    break;
            }
        }

        private static void Generate(CreateHyperTableOperation operation, IndentedStringBuilder builder)
        {
            builder.CreateSqlStatement($"SELECT create_hypertable( '\\\"{operation.TableName}\\\"', by_range('{operation.HyperProperty.Name}'));");
            if (operation.Retention is not null)
            {
                builder.CreateSqlStatement($"SELECT add_retention_policy( '\\\"{operation.TableName}\\\"', INTERVAL '{operation.Retention}');", withBuilder: true);
            }
        }

        private static void Generate(DropHyperTableOperation operation, IndentedStringBuilder builder)
        {
            builder.CreateSqlStatement("SELECT 'NOOP' as Noop;");
            if (operation.Retention is not null)
            {
                builder.CreateSqlStatement("SELECT 'NOOP' as Noop;",  description: $"SELECT remove_retention_policy( '\\\"{operation.Name}\\\"');", withBuilder: true);
            }
        }
    }
}
