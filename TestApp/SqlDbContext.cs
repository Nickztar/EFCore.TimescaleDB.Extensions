using EFCore.TimescaleDB.Extensions;
using EFCore.TimescaleDB.Extensions.Annotations;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    public class SqlDbContext : DbContext
    {
        public DbSet<WeatherForecast> Forecasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherForecast>().IsHyperTable(nameof(WeatherForecast.Date), retentionInterval: "8 days", chunkSize: "1 day");
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql("Host=localhost:5432;Database=postgres;Username=postgres;Password=password")
                .ConfigureTimescale();
            base.OnConfiguring(optionsBuilder);
        }
        
        
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
            => configurationBuilder.AddHyperTableConfiguration();
    }
}
