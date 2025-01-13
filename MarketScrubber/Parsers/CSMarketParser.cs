using System.Text.Json;
using buff163;
using NUnit.Framework;

namespace CSMarketBuff163SkinsParser;

public class CSMarketParser : ISellMarketParser
{
    private readonly CookiesConfig config;
    public CSMarketParser(CookiesConfig config)
    {
        this.config = config;
    }
    
    public ItemsRoot GetMostPopularItems(int volume, HttpClient client)
    {
        try
        {
            var url = config.BaseUrlSellers;
            if (url == null || url == "")
            {
                throw new Exception("Invalid base url");
            }
            var response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status code: {response.StatusCode}");
            }
            
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var items = JsonSerializer.Deserialize<ItemsRoot>(responseBody);
            
            if (items == null)
            {
                throw new Exception("Invalid parsing response");
            }
            DeleteUnnecessary(items, volume);
            return items;
        }
        catch (Exception ex)
        {
            throw new Exception($"Client error: {ex.Message}", ex);
        }
    }

    private static void DeleteUnnecessary(ItemsRoot itemsRoot, int volume)
    {
        itemsRoot.Items = itemsRoot.Items
            .AsParallel()
            .Where(item => int.TryParse(item.Volume, out var itemVolume) && itemVolume > volume)
            .ToList();
    }
}