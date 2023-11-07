namespace E8;

class AndroidLocationProvider : ILocationProvider
{
    private Point? _currentPoint;
    public DateTime LastMeasurement { get; private set; } = DateTime.MinValue;
    public Point GetLocation()
    {
        Random rnd = new Random();
        DateTime now = DateTime.Now;
        if ((now - LastMeasurement).TotalSeconds > 2)
        {
            LastMeasurement = now;
            _currentPoint = new Point(rnd.NextDouble() * 180 - 90, rnd.NextDouble() * 360);
        }
        return _currentPoint!;
    }
}