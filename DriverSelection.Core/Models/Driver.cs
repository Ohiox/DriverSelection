namespace DriverSelection.Core.Models;

/// <summary>
/// Represents a taxi driver with unique identifier and current location.
/// </summary>
public record Driver(string Id, Point Location);
