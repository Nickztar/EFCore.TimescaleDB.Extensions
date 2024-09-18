using EFCore.TimescaleDB.Extensions.Attributes;
using Microsoft.EntityFrameworkCore;

namespace TestApp;


[Keyless, HyperTable(nameof(Time), "1 day")]
public abstract class BaseClass
{
    public DateTimeOffset Time { get; set; }
}

public class RealClass : BaseClass
{
    public string? Text { get; set; }
}