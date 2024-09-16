using EFCore.TimescaleDB.Extensions.Operations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

namespace EFCore.TimescaleDB.Extensions
{
    public class NpgsqlServerHyperTableMigrationSqlGenerator(
        MigrationsSqlGeneratorDependencies dependencies,
        INpgsqlSingletonOptions npgsqlSingletonOptions)
        : NpgsqlMigrationsSqlGenerator(dependencies, npgsqlSingletonOptions)
    {
        protected override void Generate(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            //Debugger.Launch();
            switch (operation)
            {
                case CreateHyperTableOperation createoperation:
                    Generate(createoperation, builder, Dependencies.TypeMappingSource);
                    break;

                case DropHyperTableOperation dropoperation:
                    Generate(dropoperation, builder);
                    break;

                default:
                    base.Generate(operation, model, builder);
                    break;
            }
        }

        private void Generate(DropHyperTableOperation operation, MigrationCommandListBuilder builder)
        {
            //Drop hypertable
            // Dropping not actually supported? :D
            // DropTableType(operation.Name, builder);
        }

        private void Generate(CreateHyperTableOperation operation, MigrationCommandListBuilder builder, IRelationalTypeMappingSource typeMappingSource)
        {
            builder.AppendLine($"SELECT create_hypertable('\"{operation.TableName}\"', '{operation.HyperProperty.Name}');");
            builder.EndCommand();
        }
    }
}
