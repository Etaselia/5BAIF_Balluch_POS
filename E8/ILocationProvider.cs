namespace E8;

interface ILocationProvider
{
    DateTime LastMeasurement { get; }
    Point GetLocation();
}