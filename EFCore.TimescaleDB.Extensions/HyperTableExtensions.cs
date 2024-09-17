using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using EFCore.TimescaleDB.Extensions.Annotations;
using EFCore.TimescaleDB.Extensions.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.TimescaleDB.Extensions;

public static class HyperTableExtensions
{
    public static DbContextOptionsBuilder ConfigureTimescale(this DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ReplaceService<IMigrationsModelDiffer, HyperTableMigrationsModelDiffer>();
        return optionsBuilder;
    }
    
    
    public static void AddHyperTableConfiguration(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(sp => new HyperTableAttributeConvention(
            sp.GetRequiredService<ProviderConventionSetBuilderDependencies>()));
    }
    
    public static EntityTypeBuilder<T> IsHyperTable<T>(
        this EntityTypeBuilder<T> entityBuilder, 
        string hyperPropertyName,
        string? retentionInterval = null,
        string? chunkSize = null)
        where T : class
    {
        entityBuilder.HasAnnotation(Constants.HyperTable, new HyperTableConfiguration()
        {
            RetentionInterval = retentionInterval,
            ChunkSize = chunkSize,
            PropertyName = hyperPropertyName
        }.AsString());
        return entityBuilder;
    }
    
    internal static IndentedStringBuilder CreateSqlStatement(
        this IndentedStringBuilder builder, 
        string sql, 
        string? description = null, 
        bool withBuilder = false,
        bool isFinalStatement = false,
        bool tripleQuote = false)
    {
        builder.
            AppendLine($"{(withBuilder ? "migrationBuilder" : "")}.Sql(");
        using (builder.Indent())
        {
            var quotes = string.Concat(Enumerable.Repeat('\"', tripleQuote ? 3 : 1));
            var endQuote = quotes;
            if (tripleQuote)
            {
                quotes += "\n";
                endQuote = "\n" + endQuote;
            }
            builder.AppendLine(quotes + sql + endQuote + (description is null ? "" : $"//{description}"));
        }
        if (isFinalStatement) builder.Append(")");
        else builder.AppendLine($");");
        
        return builder;
    }
    
    internal static HyperTableConfiguration? GetHyperConfiguration(this IEntityType entity)
    {
        var annotationValue = entity.GetAnnotation(Constants.HyperTable)?.Value;
        return annotationValue is not string annotationStr ? null : HyperTableConfiguration.FromString(annotationStr);
    }
    
    internal static bool IsEqual(this HyperTableConfiguration? self, HyperTableConfiguration? other)
    {
        if (self is null && other is null) return true;
        if (self is null || other is null) return false;
        return self.ChunkSize == other.ChunkSize && 
            self.RetentionInterval == other.RetentionInterval && 
            self.PropertyName == other.PropertyName;
    }
    
    internal static bool IsSoftChange([NotNullWhen(true)] this HyperTableConfiguration? self, [NotNullWhen(true)] HyperTableConfiguration? other)
    {
        if (self is null && other is null) return false;
        if (self is null || other is null) return false;
        return self.PropertyName == other.PropertyName;
    }
    
    internal static HyperPropertyChanged PropertyChanged(this HyperTableConfiguration self, HyperTableConfiguration other)
    {
        var changed = HyperPropertyChanged.None; 
        if (self.RetentionInterval != other.RetentionInterval) changed |= HyperPropertyChanged.Retention;
        if (self.ChunkSize != other.ChunkSize) changed |= HyperPropertyChanged.ChunkSize;
        return changed;
    }
    
    [Flags]
    internal enum HyperPropertyChanged
    {
        None,
        Retention = 1 << 0,
        ChunkSize = 1 << 1
    }
}