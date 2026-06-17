using DriverSelection.Core.Models;

namespace DriverSelection.Core.Search;

/// <summary>
/// Maintains a list of K best drivers while scanning through all candidates.
/// More memory-efficient than sorting entire dataset for large K.
/// Note: Linear search for max element in best list - could use actual heap structure for better performance.
/// Time complexity: O(n * k) where k is count parameter.
/// </summary>
public class HeapNearestDriversFinder : INearestDriversFinder
{
    public string Name => nameof(HeapNearestDriversFinder);

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

        var best = new List<(Driver Driver, long Distance)>();

        foreach (var driver in drivers)
        {
            var distance = driver.Location.DistanceSquared(orderLocation);

            if (best.Count < count)
            {
                best.Add((driver, distance));
                continue;
            }

            var worstIndex = 0;
            var worstDistance = best[0].Distance;
            for (var i = 1; i < best.Count; i++)
            {
                if (best[i].Distance > worstDistance || (best[i].Distance == worstDistance && string.CompareOrdinal(best[i].Driver.Id, best[worstIndex].Driver.Id) > 0))
                {
                    worstIndex = i;
                    worstDistance = best[i].Distance;
                }
            }

            if (distance < worstDistance || (distance == worstDistance && string.CompareOrdinal(driver.Id, best[worstIndex].Driver.Id) < 0))
            {
                best[worstIndex] = (driver, distance);
            }
        }

        return best
            .OrderBy(item => item.Distance)
            .ThenBy(item => item.Driver.Id, StringComparer.Ordinal)
            .Select(item => item.Driver)
            .ToArray();
    }
}
