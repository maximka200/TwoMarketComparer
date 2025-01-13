using System.Text.Json;

namespace CSMarketBuff163SkinsParser;

public class CSMarketParser : ISellMarketParser
{
    public ItemsRoot GetMostPopularItems(int volume, HttpClient client, string baseUrl)
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
            DeleteUnnecessary(items, volume);
            return items;
        }
        catch (Exception ex)
        {
            throw new Exception($"{op}: Error: {ex.Message}", ex);
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