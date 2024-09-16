using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.TimescaleDB.Extensions
{
    public class DesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMigrationsCodeGenerator, CSharpHyperTableMigrationsGenerator>();
            serviceCollection.AddSingleton<ICSharpMigrationOperationGenerator, CSharpHyperTableMigrationOperationGenerator>();
        }
    }
}
