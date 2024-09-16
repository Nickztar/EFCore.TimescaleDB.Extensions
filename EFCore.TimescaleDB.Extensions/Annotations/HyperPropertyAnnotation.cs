using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.TimescaleDB.Extensions.Annotations
{
    public class HyperPropertyAnnotation : IAnnotation
    {
        public string Name => Constants.HyperProperty;

        public object? Value { get; }
    }
}
