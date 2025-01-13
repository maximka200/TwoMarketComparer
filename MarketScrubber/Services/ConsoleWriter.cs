namespace MarketScrubber.Services;

public static class ConsoleWriter
{
    public static Action SimulateLoadingBar(int total)
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
}