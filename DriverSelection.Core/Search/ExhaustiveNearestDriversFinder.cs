using DriverSelection.Core.Models;

namespace DriverSelection.Core.Search;

/// <summary>
/// Simple and straightforward algorithm - calculates distance to all drivers and sorts.
/// Works well for small datasets (< 1000 drivers).
/// Time complexity: O(n log n) due to sorting.
/// </summary>
public class ExhaustiveNearestDriversFinder : INearestDriversFinder
{
    public string Name => nameof(ExhaustiveNearestDriversFinder);

    public IReadOnlyList<Driver> FindNearestDrivers(IEnumerable<Driver> drivers, Point orderLocation, int count = 5)
    {
        if (drivers is null)
        {
            throw new ArgumentNullException(nameof(drivers));
        }

        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than zero.");
        }

        return drivers
            .Select(driver => new { Driver = driver, Distance = driver.Location.DistanceSquared(orderLocation) })
            .OrderBy(item => item.Distance)
            .ThenBy(item => item.Driver.Id, StringComparer.Ordinal)
            .Take(count)
            .Select(item => item.Driver)
            .ToArray();
    }
}
