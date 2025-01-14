namespace CSMarketBuff163SkinsParser;

public interface IBuyMarketParser
{
    public Task<Buyer?> GetItemByNameAsync(string name, HttpClient client, string baseUrl);
}