using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EFCore.TimescaleDB.Extensions.Annotations
{
    public class HyperTableAnnotation : IAnnotation
    {
        public string Name => Constants.HyperTable;
        public required HyperTableConfiguration Configuration { get; set; }
        public object? Value => Configuration;
    }
    
    public class HyperTableConfiguration
    {
        public string? RetentionInterval { get; set; }
        public string? ChunkSize { get; set; }
        public required string PropertyName { get; set; }
        
        public string AsString()
        {
            return $"{PropertyName};{ChunkSize};{RetentionInterval}";
        }
        
        public static HyperTableConfiguration? FromString(string value)
        {
            var parts = value.Split(";");
            if (parts.Length < 1) return null;
            var (propertyName, chunkSize, retentionInterval) = (
                parts[0], 
                parts.ElementAtOrDefault(1), 
                parts.ElementAtOrDefault(2)
            );
            return new HyperTableConfiguration()
            {
                PropertyName = propertyName,
                ChunkSize = string.IsNullOrEmpty(chunkSize) ? null : chunkSize,
                RetentionInterval = string.IsNullOrEmpty(retentionInterval) ? null : retentionInterval
            };
        }
    }
}
