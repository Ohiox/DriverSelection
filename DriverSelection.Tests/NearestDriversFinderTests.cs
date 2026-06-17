using DriverSelection.Core.Models;
using DriverSelection.Core.Search;

namespace DriverSelection.Tests;

/// <summary>
/// Test suite for nearest drivers finder implementations.
/// All implementations must return the same results for correctness.
/// </summary>
public class NearestDriversFinderTests
{
    private readonly IReadOnlyList<Driver> _drivers = new[]
    {
        new Driver("A", new Point(0, 0)),
        new Driver("B", new Point(1, 1)),
        new Driver("C", new Point(2, 2)),
        new Driver("D", new Point(3, 3)),
        new Driver("E", new Point(4, 4)),
        new Driver("F", new Point(5, 5)),
    };

    [Test]
    [Description("Verify ExhaustiveNearestDriversFinder returns correct top 3 nearest drivers")]
    public void ExhaustiveFinder_ReturnsNearestDrivers()
    {
        var finder = new ExhaustiveNearestDriversFinder();
        var nearest = finder.FindNearestDrivers(_drivers, new Point(2, 1), 3);

        Assert.That(nearest.Select(driver => driver.Id), Is.EqualTo(new[] { "B", "C", "A" }));
    }

    [Test]
    public void HeapFinder_ReturnsNearestDrivers()
    {
        var finder = new HeapNearestDriversFinder();
        var nearest = finder.FindNearestDrivers(_drivers, new Point(2, 1), 3);

        // Must match ExhaustiveNearestDriversFinder results for validation
        Assert.That(nearest.Select(driver => driver.Id), Is.EqualTo(new[] { "B", "C", "A" }));
    }

    [Test]
    public void GridFinder_ReturnsNearestDrivers()
    {
        var finder = new GridNearestDriversFinder(2);
        var nearest = finder.FindNearestDrivers(_drivers, new Point(2, 1), 3);

        Assert.That(nearest.Select(driver => driver.Id), Is.EqualTo(new[] { "B", "C", "A" }));
    }

    [Test]
    public void FindNearestDrivers_ThrowsOnNullDrivers()
    {
        var finder = new ExhaustiveNearestDriversFinder();

        var action = () => finder.FindNearestDrivers(null!, new Point(0, 0), 1);
        Assert.That(action, Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void FindNearestDrivers_ThrowsOnInvalidCount()
    {
        var finder = new ExhaustiveNearestDriversFinder();

        var action = () => finder.FindNearestDrivers(_drivers, new Point(0, 0), 0);
        Assert.That(action, Throws.InstanceOf<ArgumentOutOfRangeException>());
    }
}
