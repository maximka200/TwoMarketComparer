namespace CSMarketBuff163SkinsParser;

public interface ISellMarketParser
{
    public ItemsRoot GetMostPopularItems(int count, float price, HttpClient client, string baseUrl);
}