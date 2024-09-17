using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EFCore.TimescaleDB.Extensions.Operations;

public class DropChunkSizeOperation : MigrationOperation
{
    public DropChunkSizeOperation(string name, string chunkSize)
    {
        Name = name;
        ChunkSize = chunkSize;
    }

    public string Name { get; }
    public string ChunkSize { get; }
}