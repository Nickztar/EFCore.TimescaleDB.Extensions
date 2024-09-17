using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EFCore.TimescaleDB.Extensions.Operations
{
    public class CreateHyperTableOperation : MigrationOperation
    {
        public CreateHyperTableOperation(string tableName, IColumn hyperProperty, string? retention, string? chunkSize)
        {
            TableName = tableName;
            HyperProperty = hyperProperty;
            Retention = retention;
            ChunkSize = chunkSize;
        }

        public string TableName { get; }

        public IColumn HyperProperty { get; }
        public string? Retention { get; }
        public string? ChunkSize { get; }
    }
}
