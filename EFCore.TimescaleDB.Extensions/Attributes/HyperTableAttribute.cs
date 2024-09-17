using EFCore.TimescaleDB.Extensions.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace EFCore.TimescaleDB.Extensions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class HyperTableAttribute(string propertyName, string? retentionInterval = null, string? chunkSize = null) : Attribute
{
    public readonly string PropertyName = propertyName;
    public readonly string? RetentionInterval = retentionInterval;
    public readonly string? ChunkSize = chunkSize;
}

public class HyperTableAttributeConvention(ProviderConventionSetBuilderDependencies dependencies) :
    TypeAttributeConventionBase<HyperTableAttribute>(dependencies)
{
    protected override void ProcessEntityTypeAdded(
        IConventionEntityTypeBuilder entityTypeBuilder,
        HyperTableAttribute attribute,
        IConventionContext<IConventionEntityTypeBuilder> context)
    {
        var property = entityTypeBuilder.Metadata.FindProperty(attribute.PropertyName);
        if (property is null)
            throw new Exception("Hypertable property not found");
        if (property.ClrType != typeof(DateTime) && property.ClrType != typeof(DateTimeOffset))
            throw new Exception("Hypertable property is not a timestamp or timestampz");
        entityTypeBuilder.HasAnnotation(Constants.HyperTable, new HyperTableConfiguration()
        {
            RetentionInterval = attribute.RetentionInterval,
            ChunkSize = attribute.ChunkSize,
            PropertyName = property.Name
        }.AsString());
    }
}