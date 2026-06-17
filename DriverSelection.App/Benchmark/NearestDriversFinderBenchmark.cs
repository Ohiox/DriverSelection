using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DriverSelection.Core.Models;
using DriverSelection.Core.Search;

namespace DriverSelection.App.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class NearestDriversFinderBenchmark
{
    private List<Driver> _drivers = null!;
    private Point _orderLocation;

    [Params(1000, 5000, 20000)]
    public int DriverCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _orderLocation = new Point(500, 500);
        _drivers = Enumerable.Range(0, DriverCount)
            .Select(index => new Driver(index.ToString(), new Point(random.Next(0, 1000), random.Next(0, 1000))))
            .ToList();
    }

    [Benchmark(Baseline = true)]
    public IReadOnlyList<Driver> Exhaustive() =>
        new ExhaustiveNearestDriversFinder().FindNearestDrivers(_drivers, _orderLocation, 5);

    [Benchmark]
    public IReadOnlyList<Driver> Heap() =>
        new HeapNearestDriversFinder().FindNearestDrivers(_drivers, _orderLocation, 5);

    [Benchmark]
    public IReadOnlyList<Driver> Grid() =>
        new GridNearestDriversFinder(25).FindNearestDrivers(_drivers, _orderLocation, 5);
}
