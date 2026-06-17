namespace DriverSelection.Core.Models;

/// <summary>
/// Represents a point on a rectangular grid.
/// </summary>
public readonly record struct Point(int X, int Y)
{
    /// <summary>
    /// Calculates squared Euclidean distance to another point.
    /// Using squared distance avoids expensive sqrt() calls and is sufficient for comparison.
    /// </summary>
    public long DistanceSquared(Point other)
        => (long)(X - other.X) * (X - other.X) + (long)(Y - other.Y) * (Y - other.Y);
}
