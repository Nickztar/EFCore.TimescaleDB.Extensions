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
            => DiffCollection(source, target, diffContext, Diff, Add, Remove, (x, y, diff) => x.Name.Equals(y.Name, StringComparison.CurrentCultureIgnoreCase));

        private IEnumerable<MigrationOperation> Remove(IEntityType source, DiffContext context)
        {
            var targetProperty = source.GetProperties().FirstOrDefault(p => p.GetAnnotations().Any(annotation => annotation.Name.Equals(Constants.HyperProperty)));
            var retentionValue = targetProperty?.GetAnnotation(Constants.HyperProperty)?.Value as string;
            yield return new DropHyperTableOperation(source.GetTableName(), retentionValue);
        }

        private IEnumerable<MigrationOperation> Add(IEntityType target, DiffContext context)
        {
            var targetProperty = target.GetProperties().FirstOrDefault(p => p.GetAnnotations().Any(annotation => annotation.Name.Equals(Constants.HyperProperty)));
            var retentionValue = targetProperty?.GetAnnotation(Constants.HyperProperty)?.Value as string;
            foreach (var table in target.GetTableMappings())
            {
                var hyperProperty = table.Table.Columns.FirstOrDefault(x => x.Name == targetProperty?.GetColumnName());
                if (hyperProperty is null) continue;
                yield return new CreateHyperTableOperation(target.GetTableName(), hyperProperty, retentionValue);
            }
        }

        private IEnumerable<MigrationOperation> Diff(IEntityType source, IEntityType target, DiffContext context)
        {
            if (source == target)
            {
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
            return entityTypes.Where(x => x.GetProperties().Any(p => p.GetAnnotations().Any(annotation => annotation.Name.Equals(Constants.HyperProperty)))).ToList();
        }
    }
}
#pragma warning restore EF1001
