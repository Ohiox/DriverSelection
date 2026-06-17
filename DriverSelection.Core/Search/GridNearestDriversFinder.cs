using DriverSelection.Core.Models;

namespace DriverSelection.Core.Search;

/// <summary>
/// Spatial partitioning approach - divides map into grid cells.
/// Most efficient for large datasets with uniform distribution.
/// Searches from center outward, expanding radius until enough candidates found.
/// Performance heavily depends on cellSize parameter and driver distribution.
/// </summary>
public class GridNearestDriversFinder : INearestDriversFinder
{
    private readonly int _cellSize;

    public GridNearestDriversFinder(int cellSize = 10)
    {
        if (cellSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cellSize), "Cell size must be positive.");
        }

        _cellSize = cellSize;
    }

    public string Name => nameof(GridNearestDriversFinder);

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

        var buckets = new Dictionary<(int X, int Y), List<Driver>>();

        foreach (var driver in drivers)
        {
            var bucket = (X: GetCellCoordinate(driver.Location.X), Y: GetCellCoordinate(driver.Location.Y));
            if (!buckets.TryGetValue(bucket, out var list))
            {
                list = new List<Driver>();
                buckets[bucket] = list;
            }

            list.Add(driver);
        }

        var centerCell = (X: GetCellCoordinate(orderLocation.X), Y: GetCellCoordinate(orderLocation.Y));
        var candidates = new List<(Driver Driver, long Distance)>();
        var visitedCells = new HashSet<(int X, int Y)>();
        var searchRadius = 0;

        // Expand search radius from center cell until we have enough candidates
        // TODO: Consider using PriorityQueue instead of List to avoid duplicate checks
        while (candidates.Count < count)
        {
            foreach (var cell in GetCellsInRadius(centerCell, searchRadius))
            {
                if (!visitedCells.Add(cell))
                {
                    continue;
                }

                if (buckets.TryGetValue(cell, out var driversInCell))
                {
                    candidates.AddRange(driversInCell.Select(driver => (driver, driver.Location.DistanceSquared(orderLocation))));
                }
            }

            // Safety check to prevent infinite loop for sparse datasets
            if (searchRadius > 1000 && candidates.Count == 0)
            {
                break;
            }

            searchRadius++;
        }

        return candidates
            .OrderBy(item => item.Distance)
            .ThenBy(item => item.Driver.Id, StringComparer.Ordinal)
            .Take(count)
            .Select(item => item.Driver)
            .ToArray();
    }

    private int GetCellCoordinate(int value)
        => value / _cellSize;

    private static IEnumerable<(int X, int Y)> GetCellsInRadius((int X, int Y) center, int radius)
    {
        if (radius == 0)
        {
            yield return center;
            yield break;
        }

        var startX = center.X - radius;
        var endX = center.X + radius;
        var startY = center.Y - radius;
        var endY = center.Y + radius;

        for (var x = startX; x <= endX; x++)
        {
            for (var y = startY; y <= endY; y++)
            {
                if (Math.Abs(x - center.X) == radius || Math.Abs(y - center.Y) == radius)
                {
                    yield return (x, y);
                }
            }
        }
    }
}
