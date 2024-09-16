### EFCore.TimescaleDB.Extensions


## NOTE: Not affiliated in anyway with Timescale DB nor is this super well made, but works for most cases.


## Installation (SQL Server)

1. Add `EFCore.TimescaleDB.Extensions` reference to your project.
2. Add the following attribute to your **startup/data** project:

```csharp
[assembly: DesignTimeServicesReference("EFCore.TimescaleDB.Extensions.DesignTimeServices, EFCore.TimescaleDB.Extensions")]
```

3. Use the `.ConfigureTimescale()` extension of the `DbContextOptionsBuilder`, e.g.:

```csharp
var host = Host.CreateDefaultBuilder(args)
               .ConfigureServices(
                   services =>
                   {
                       services.AddDbContext<ApplicationDbContext>(
                           options =>
                               options.UseNpgsql("<connection-string>")
                                      .ConfigureTimescale());
                   }).Build();
```

4. Use the `IsHyperProperty(...)` extension of the `PropertyBuilder` to select the entities which should be audited:

```csharp

[Keyless]
public class Product 
{
    public string Name { get; set; }
    public DateTimeOffset Time { get; set; }
}

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
        => modelBuilder.Entity<Product>().Property(x => x.Time).IsHyperProperty(retentionInterval: "24 hours"); // Use optional retention interval
}
```

5. OR: Add support for configuring via the HyperTable attribute/convention by using the `AddHyperTableConfiguration()` extension on `ModelConfigurationBuilder` 
```csharp
[Keyless]
[HyperTable(nameof(Time), "24 hours")] // Optional retention interval
public class Product 
{
    public string Name { get; set; }
    public DateTimeOffset Time { get; set; }
}

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        => configurationBuilder.AddHyperTableConfiguration();
}
```

6. Create a migration and update the database.

7. Due to the way HyperTables works, there will be no Down command generated. To use those, either rewrite the down method. In the retention case I have added the command required as comment which can be used, but needs the table to still exist.