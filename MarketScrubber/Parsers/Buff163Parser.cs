using System.Net;
using System.Text.Json;
using MarketScrubber.Services;

namespace CSMarketBuff163SkinsParser;

public class Buff163Parser : IBuyMarketParser
{
    private readonly Config config;
    
    private const string UrlPiece = "game=csgo&page_num=1&search=";
    
    private const string MinPriceProperty = "buy_max_price";

    public async Task<Buyer?> GetItemByNameAsync(string name, HttpClient client, string baseUrl)
    {
        var op = nameof(GetItemByNameAsync);
        try
        {
            if (baseUrl == null || baseUrl == "")
            {
                throw new Exception("Invalid base url");
            }

            var queryParams = $"{UrlPiece}{Uri.EscapeDataString(name)}";
            var requestUrl = $"{baseUrl}?{queryParams}";

            var response = client.GetAsync(requestUrl).Result;
            if (response.StatusCode == (HttpStatusCode)429)
            {
                Thread.Sleep(1000);
                response = client.GetAsync(requestUrl).Result;
            }
            else if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseText = await response.Content.ReadAsStringAsync();
            var price = GetMinPriceFromJson(responseText, name);
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
    
    private static string? GetMinPriceFromJson(string jsonResponse, string name)
    {
        var op = nameof(GetMinPriceFromJson);
        try
        {
            var root = JsonDocument.Parse(jsonResponse).RootElement;

            if (root.TryGetProperty("data", out var data) && data.TryGetProperty("items", out var items) &&
                items.GetArrayLength() > 0)
            {
                JsonElement targetItem = items[0];

                foreach (var item in items.EnumerateArray())
                {
                    if (item.TryGetProperty("name", out var nameInBuyer) && nameInBuyer.GetString() == name)
                    {   
                        targetItem = item;
                        break;
                    }
                }

                if (targetItem.TryGetProperty(MinPriceProperty, out var buyMaxPrice))
                {
                    return buyMaxPrice.GetString();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"{op}: Error serialize JSON: {ex.Message}");
        }

        return null;
    }
}