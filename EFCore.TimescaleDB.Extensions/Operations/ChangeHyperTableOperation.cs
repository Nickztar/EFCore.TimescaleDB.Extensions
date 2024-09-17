using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EFCore.TimescaleDB.Extensions.Operations;

public class ChangeHyperTableOperation : MigrationOperation
{
    public string TableName { get; init; }
    public string? Retention { get; init; }
    public string? ChunkSize { get; init; }
}