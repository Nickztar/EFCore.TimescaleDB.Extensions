using System.Diagnostics;
using EFCore.TimescaleDB.Extensions.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace EFCore.TimescaleDB.Extensions
{
#pragma warning disable EF1001
    public class HyperTableMigrationsModelDiffer : MigrationsModelDiffer
    {
        public HyperTableMigrationsModelDiffer(IRelationalTypeMappingSource typeMappingSource,
            IMigrationsAnnotationProvider migrationsAnnotationProvider,
            IRowIdentityMapFactory rowIdentityMapFactory,
            CommandBatchPreparerDependencies commandBatchPreparerDependencies)
            : base(typeMappingSource, migrationsAnnotationProvider, rowIdentityMapFactory, commandBatchPreparerDependencies)
        {
        }

        public override IReadOnlyList<MigrationOperation> GetDifferences(IRelationalModel? source, IRelationalModel? target)
        {
            // Debugger.Launch();
            var sourceTypes = GetEntityTypesContainingHyperPropertyAnnotation(source);
            var targetTypes = GetEntityTypesContainingHyperPropertyAnnotation(target);

            var diffContext = new DiffContext();
            var mergeMigrationOperations = Diff(sourceTypes, targetTypes, diffContext);
            return base.GetDifferences(source, target).Concat(mergeMigrationOperations).ToList();
        }

        private IEnumerable<MigrationOperation> Diff(
            IEnumerable<IEntityType> source,
            IEnumerable<IEntityType> target,
            DiffContext diffContext)
            => DiffCollection(
                source, 
                target, 
                diffContext, 
                Diff, 
                Add, 
                Remove, 
                (x, y, diff) => x.Name == y.Name
            );

        private IEnumerable<MigrationOperation> Remove(IEntityType source, DiffContext context)
        {
            var configuration = source.GetHyperConfiguration();
            if (configuration?.RetentionInterval != null)
                yield return new DropHyperRetentionOperation(source.GetTableName(), configuration.RetentionInterval);
        }
        
        private IEnumerable<MigrationOperation> SoftRemove(IEntityType source, HyperTableExtensions.HyperPropertyChanged changed)
        {
            var configuration = source.GetHyperConfiguration();
            if (changed.HasFlag(HyperTableExtensions.HyperPropertyChanged.Retention) && configuration?.RetentionInterval != null)
                yield return new DropHyperRetentionOperation(source.GetTableName(), configuration.RetentionInterval);
        }

        private IEnumerable<MigrationOperation> Add(IEntityType target, DiffContext context)
        {
            var hyperTableConfiguration = target.GetHyperConfiguration();
            if (hyperTableConfiguration is null) yield break;
            var property = target.GetProperty(hyperTableConfiguration.PropertyName);
            foreach (var table in target.GetTableMappings())
            {
                var hyperProperty = table.Table.Columns.FirstOrDefault(x => x.Name == property?.GetColumnName());
                if (hyperProperty is null) continue;
                yield return new CreateHyperTableOperation(
                    target.GetTableName(), 
                    hyperProperty, 
                    hyperTableConfiguration.RetentionInterval, 
                    hyperTableConfiguration.ChunkSize);
            }
        }
        
        private IEnumerable<MigrationOperation> SoftAdd(IEntityType target, HyperTableExtensions.HyperPropertyChanged changed)
        {
            var hyperTableConfiguration = target.GetHyperConfiguration();
            if (hyperTableConfiguration is null) yield break;
            var property = target.GetProperty(hyperTableConfiguration.PropertyName);
            foreach (var table in target.GetTableMappings())
            {
                var hyperProperty = table.Table.Columns.FirstOrDefault(x => x.Name == property?.GetColumnName());
                if (hyperProperty is null) continue;
                yield return new ChangeHyperTableOperation
                {
                   TableName = target.GetTableName(),
                   Retention = changed.HasFlag(HyperTableExtensions.HyperPropertyChanged.Retention) ? hyperTableConfiguration.RetentionInterval : null,
                   ChunkSize = changed.HasFlag(HyperTableExtensions.HyperPropertyChanged.ChunkSize) ? hyperTableConfiguration.ChunkSize : null,
                };
            }
        }

        private IEnumerable<MigrationOperation> Diff(IEntityType source, IEntityType target, DiffContext context)
        {
            var sourceConfiguration = source.GetHyperConfiguration();
            var targetConfiguration = target.GetHyperConfiguration();
            if (sourceConfiguration.IsEqual(targetConfiguration))
            {
                yield break;
            }
            
            if (sourceConfiguration.IsSoftChange(targetConfiguration))
            {
                var changed = sourceConfiguration.PropertyChanged(targetConfiguration);
                if (changed.HasFlag(HyperTableExtensions.HyperPropertyChanged.Retention))
                {
                    var softDrops = SoftRemove(source, changed);
                    foreach (var operation in softDrops)
                    {
                        yield return operation;
                    }
                }
                
                var softAdds = SoftAdd(target, changed);
                foreach (var operation in softAdds)
                {
                    yield return operation;
                }
                
                yield break;
            }
            
            var dropOperations = Remove(source, context);
            foreach (var operation in dropOperations)
            {
                yield return operation;
            }
            
            var addOperations = Add(target, context);
            foreach (var operation in addOperations)
            {
                yield return operation;
            }
        }

        private static List<IEntityType>? GetEntityTypesContainingHyperPropertyAnnotation(IRelationalModel? relationalModel)
        {
            if (relationalModel == null)
            {
                return new List<IEntityType>();
            }

            var entityTypes = relationalModel.Model.GetEntityTypes();
            return entityTypes.Where(x => x.GetAnnotations().Any(a => a.Name == Constants.HyperTable)).ToList();
        }
    }
}
#pragma warning restore EF1001
