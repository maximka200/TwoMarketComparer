namespace MarketScrubber.Services;

public static class ConsoleWriter
{
    public static Action SimulateLoadingBarV1(int total)
    {
        var counter = 0;
        Console.Write("[");
        return () =>
        {
            if (total == counter)
            {
                Console.Write("#]");
            }
            else
            {
                counter++;
                Console.Write('#');
            }
        };
    }
    
    public static void SimulateLoadingBarV2(int length, int ready)
    {
        string filled = new string('#', ready);
        string unfilled = new string('-', length - ready);
    
        Console.Write($"\r[{filled}{unfilled}] {ready * 100 / length}%");
    }
}