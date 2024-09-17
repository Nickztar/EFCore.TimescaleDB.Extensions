using Microsoft.EntityFrameworkCore.Migrations.Design;

namespace EFCore.TimescaleDB.Extensions
{
    public class CSharpHyperTableMigrationsGenerator(
        MigrationsCodeGeneratorDependencies dependencies,
        CSharpMigrationsGeneratorDependencies csharpDependencies)
        : Microsoft.EntityFrameworkCore.Migrations.Design.CSharpMigrationsGenerator(dependencies, csharpDependencies);
}
