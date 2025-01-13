using System.Text.Json.Serialization;

namespace CSMarketBuff163SkinsParser;

public class Item
{
    [JsonPropertyName("market_hash_name")] 
    public string MarketHashName { get; set; } = "-";

    [JsonPropertyName("volume")] 
    public string Volume { get; set; } = "0";

    [JsonPropertyName("price")] 
    public string Price { get; set; } = "0";

    [JsonPropertyName("url")] 
    public string UrlSell { get; set; } = "-";
}

public class ItemsRoot
{
    [JsonPropertyName("items")]
    public List<Item> Items { get; set; }
}

