using System.ComponentModel.DataAnnotations.Schema;
using EFCore.TimescaleDB.Extensions.Attributes;
using Microsoft.EntityFrameworkCore;

namespace TestApp;
// [HyperTable(nameof(Date), "7 days", "7 days")]
[Keyless]
public class WeatherForecast
{
    public WeatherForecast()
    {

    }

    public WeatherForecast(DateTime date, int temperatureC, string summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
