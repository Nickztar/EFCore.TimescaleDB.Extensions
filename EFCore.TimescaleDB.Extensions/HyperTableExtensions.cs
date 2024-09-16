using EFCore.TimescaleDB.Extensions.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        optionsBuilder.ReplaceService<IMigrationsSqlGenerator, NpgsqlServerHyperTableMigrationSqlGenerator>();
        return optionsBuilder;
    }
    
    
    public static void AddHyperTableConfiguration(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(sp => new HyperTableAttributeConvention(
            sp.GetRequiredService<ProviderConventionSetBuilderDependencies>()));
    }
    
    public static PropertyBuilder<T> IsHyperProperty<T>(this PropertyBuilder<T> propertyBuilder, string? retentionInterval = null)
    {
        propertyBuilder.HasAnnotation(Constants.HyperProperty, retentionInterval);
        return propertyBuilder;
    }
    
    internal static IndentedStringBuilder CreateSqlStatement(this IndentedStringBuilder builder, string sql, string? description = null, bool withBuilder = false)
    {
        builder.
            AppendLine($"{(withBuilder ? "migrationBuilder" : "")}.Sql(");
        using (builder.Indent())
        {
            builder.AppendLine("\"" + sql + "\"" + (description is null ? "" : $"//{description}"));
        }
        builder.AppendLine($");");
        
        return builder;
    }
}