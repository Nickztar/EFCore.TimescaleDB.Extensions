using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EFCore.TimescaleDB.Extensions.Operations
{
    public class CreateHyperTableOperation : MigrationOperation
    {
        public CreateHyperTableOperation(string tableName, IColumn hyperProperty, string? retention)
        {
            TableName = tableName;
            HyperProperty = hyperProperty;
            Retention = retention;
        }

        public string TableName { get; }

        public IColumn HyperProperty { get; }
        public string? Retention { get; }
    }
}
