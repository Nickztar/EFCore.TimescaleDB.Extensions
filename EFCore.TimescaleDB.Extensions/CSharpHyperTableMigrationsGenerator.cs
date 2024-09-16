using Microsoft.EntityFrameworkCore.Migrations.Design;

namespace EFCore.TimescaleDB.Extensions
{
    public class CSharpHyperTableMigrationsGenerator : Microsoft.EntityFrameworkCore.Migrations.Design.CSharpMigrationsGenerator
    {
        public CSharpHyperTableMigrationsGenerator(MigrationsCodeGeneratorDependencies dependencies, CSharpMigrationsGeneratorDependencies csharpDependencies) : base(dependencies, csharpDependencies)
        {
        }
    }
}
