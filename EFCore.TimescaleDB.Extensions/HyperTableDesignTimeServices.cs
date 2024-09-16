using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.TimescaleDB.Extensions
{
    public class HyperTableDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            //Debugger.Launch();
            serviceCollection.AddSingleton<IMigrationsCodeGenerator, CSharpHyperTableMigrationsGenerator>();
            serviceCollection.AddSingleton<ICSharpMigrationOperationGenerator, CSharpHyperTableMigrationOperationGenerator>();
        }
    }
}
