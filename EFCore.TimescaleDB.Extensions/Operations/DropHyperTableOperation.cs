using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EFCore.TimescaleDB.Extensions.Operations
{
    public class DropHyperTableOperation : MigrationOperation
    {
        public DropHyperTableOperation(string name, string? retention)
        {
            Name = name;
            Retention = retention;
        }

        public string Name { get; }
        public string? Retention { get; }
    }
}
