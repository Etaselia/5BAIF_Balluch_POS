using System;
namespace E8;
class Program
{
    public static void Main(string[] args)
    {
        MyTracker tracker;
        Console.WriteLine("Tracking mit ANDROID (alle 2 Messungen ein neuer Wert):");
        tracker = new MyTracker(new AndroidLocationProvider());
        tracker.Logger = new ConsoleLogger();
        tracker.DoTracking(10);
        Console.WriteLine("Tracking mit APPLE (alle 5 Messungen ein neuer Wert):");
        tracker = new MyTracker(new AppleLocationProvider());
        tracker.Logger = new FileLogger("locations.txt");
        tracker.DoTracking(10);
    }
}
