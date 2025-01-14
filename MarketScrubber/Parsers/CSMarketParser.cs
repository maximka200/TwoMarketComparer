using System.Globalization;
using System.Text.Json;

namespace CSMarketBuff163SkinsParser;

public class CSMarketParser : ISellMarketParser
{
    public ItemsRoot GetMostPopularItems(int volume, float price, HttpClient client, string baseUrl)
    {
        var op = System.Reflection.MethodBase.GetCurrentMethod().Name;
        try
        {
            if (baseUrl == null || baseUrl == "")
            {
                throw new Exception("Invalid base url");
            }
            var response = client.GetAsync(baseUrl).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Unexpected status code: {response.StatusCode}");
            }
            
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var items = JsonSerializer.Deserialize<ItemsRoot>(responseBody);
            
            if (items == null)
            {
                throw new Exception("Invalid parsing response");
            }
            DeleteUnnecessaryVolume(items, volume);
            DeleteUnnecessaryPrice(items, price);
            return items;
        }
        catch (Exception ex)
        {
            throw new Exception($"{op}: Error: {ex.Message}", ex);
        }
    }
    
    private static void DeleteUnnecessaryPrice(ItemsRoot itemsRoot, float price)
    {
        itemsRoot.Items = itemsRoot.Items
            .AsParallel()
            .Where(item => float.TryParse(item.Price, CultureInfo.InvariantCulture, out var itemPrice) && itemPrice > price)
            .ToList();
    }

    private static void DeleteUnnecessaryVolume(ItemsRoot itemsRoot, int volume)
    {
        itemsRoot.Items = itemsRoot.Items
            .AsParallel()
            .Where(item => int.TryParse(item.Volume, CultureInfo.InvariantCulture, out var itemVolume) && itemVolume > volume)
            .ToList();
    }
}