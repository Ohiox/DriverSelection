using BenchmarkDotNet.Running;
using DriverSelection.App.Benchmark;
using DriverSelection.Core.Models;
using DriverSelection.Core.Search;

Console.WriteLine("Driver Selection Sample Application\n");

if (args.Length > 0 && args[0].Equals("benchmark", StringComparison.OrdinalIgnoreCase))
{
    BenchmarkRunner.Run<NearestDriversFinderBenchmark>();
    return;
}

var drivers = new List<Driver>
{
    new("A", new Point(5, 5)),
    new("B", new Point(10, 10)),
    new("C", new Point(4, 8)),
    new("D", new Point(7, 4)),
    new("E", new Point(3, 2)),
    new("F", new Point(9, 9)),
    new("G", new Point(6, 6)),
};

var orderLocation = new Point(5, 7);
var finders = new INearestDriversFinder[]
{
    new ExhaustiveNearestDriversFinder(),
    new HeapNearestDriversFinder(),
    new GridNearestDriversFinder(5),
};

foreach (var finder in finders)
{
    Console.WriteLine($"Algorithm: {finder.Name}");
    var nearest = finder.FindNearestDrivers(drivers, orderLocation, 5);
    foreach (var driver in nearest)
    {
        Console.WriteLine($"  {driver.Id} at ({driver.Location.X}, {driver.Location.Y})");
    }
    Console.WriteLine();
}

Console.WriteLine("\nTo run comprehensive benchmark use:");
Console.WriteLine("  dotnet run -c Release --project DriverSelection.App -- benchmark");
Console.WriteLine("\n(Release mode recommended for accurate performance measurements)");
