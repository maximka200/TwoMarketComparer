namespace CSMarketBuff163SkinsParser;

public interface IBuyMarketParser
{
    public Buyer? GetItemByName(string name, HttpClient client, string baseUrl);
}