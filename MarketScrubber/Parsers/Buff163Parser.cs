using System.Text.Json;
using CSMarketBuff163SkinsParser;

namespace buff163;

public class Buff163Parser : IBuyMarketParser
{
    private readonly CookiesConfig config;
    
    public Buff163Parser(CookiesConfig config)
    {
        this.config = config;
    }

    public Buyer GetItemByName(string name, HttpClient client)
    {
        var queryParams = $"game=csgo&page_num=1&search={Uri.EscapeDataString(name)}";
        var requestUrl = $"{config.BaseUrlBuyers}?{queryParams}";

        try
        {
            var response = client.GetAsync(requestUrl).Result;
            var responseText = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(name);
            var price = GetMinPriceFromJson(responseText);
            return new Buyer
            {
                Name = name,
                Price = price
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Client error: {ex.Message}", ex);
        }
    }
    
    private static string GetMinPriceFromJson(string jsonResponse)
    {
        var op = "GetMinPriceFromJson";
        try
        {
            var root = JsonDocument.Parse(jsonResponse).RootElement;
        
            if (root.TryGetProperty("data", out var data) && data.TryGetProperty("items", out var items) &&
                items.GetArrayLength() > 0)
            {
                var firstItem = items[0];

                if (firstItem.TryGetProperty("sell_min_price", out var buyMaxPrice))
                {
                    return buyMaxPrice.GetString();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{op}: Error serialize JSON: {ex.Message}");
        }

        return null;
    }
}