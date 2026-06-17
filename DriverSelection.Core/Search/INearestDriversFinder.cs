using DriverSelection.Core.Models;

namespace DriverSelection.Core.Search;

/// <summary>
/// Interface for different nearest driver finding strategies.
/// Different implementations optimize for different scenarios (small/large datasets, etc).
/// </summary>
public interface INearestDriversFinder
{
    /// <summary>Gets the name of the algorithm.</summary>
    string Name { get; }

    /// <summary>
    /// Finds the nearest drivers to the order location.
    /// </summary>
    /// <param name="drivers">Collection of available drivers</param>
    /// <param name="orderLocation">The order's location</param>
    /// <param name="count">Number of nearest drivers to return (default: 5)</param>
    /// <returns>Ordered list of nearest drivers</returns>
    IReadOnlyList<Driver> FindNearestDrivers(IEnumerable<Driver> drivers, Point orderLocation, int count = 5);
}
