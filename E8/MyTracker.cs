namespace E8;

class MyTracker
{
    private readonly ILocationProvider locator;
    public ILogger Logger { get; set; }

    public MyTracker(ILocationProvider locator)
    {
        this.locator = locator;
    }

    public void DoTracking(int seconds)
    {
        DateTime start = DateTime.Now;
        while ((DateTime.Now - start).TotalSeconds < seconds)
        {
            Point position = locator.GetLocation();
            Logger?.Log($"Lat: {position.Lat:0.00}°, Lng: {position.Lng:0.00}°");
            System.Threading.Thread.Sleep(1000);
        }
    }
}