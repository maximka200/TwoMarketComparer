using System.Text.Json;
using CSMarketBuff163SkinsParser;

namespace buff163;

public class Buff163Parser : IBuyMarketParser
{
    private readonly CookiesConfig config;
    
    private const string UrlPiece = "game=csgo&page_num=1&search=";
    
    private const string MinPriceProperty = "buy_max_price";

    public Buyer? GetItemByName(string name, HttpClient client, string baseUrl)
    {
        var op = nameof(GetItemByName);
        try
        {
            if (baseUrl == null || baseUrl == "")
            {
                throw new Exception("Invalid base url");
            }

            var queryParams = $"{UrlPiece}{Uri.EscapeDataString(name)}";
            var requestUrl = $"{baseUrl}?{queryParams}";

            var response = client.GetAsync(requestUrl).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Unexpected status code: {response.StatusCode}");
            }

            var responseText = response.Content.ReadAsStringAsync().Result;
            var price = GetMinPriceFromJson(responseText);
            if (price == null)
            {
                return null;
            }
            
            return new Buyer
            {
                Name = name,
                Price = price
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"{op}: Error: {ex.Message}", ex);
        }
    }
    
    private static string? GetMinPriceFromJson(string jsonResponse)
    {
        var op = nameof(GetMinPriceFromJson);
        try
        {
            var root = JsonDocument.Parse(jsonResponse).RootElement;
        
            if (root.TryGetProperty("data", out var data) && data.TryGetProperty("items", out var items) &&
                items.GetArrayLength() > 0)
            {
                var firstItem = items[0];

                if (firstItem.TryGetProperty(MinPriceProperty, out var buyMaxPrice))
                {
                    return buyMaxPrice.GetString();
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"{op}: Error serialize JSON: {ex.Message}");
        }
    }
}