namespace CSMarketBuff163SkinsParser;

public interface ISellMarketParser
{
    public ItemsRoot GetMostPopularItems(int count, HttpClient client, string baseUrl);
}