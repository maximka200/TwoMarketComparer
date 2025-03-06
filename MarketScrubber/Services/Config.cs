using System.Text.Json;

namespace MarketScrubber.Services;

public class Config
{
    public string Cookies { get; set; }
    public string BaseUrlBuyers { get; set; }
    public string BaseUrlSellers { get; set; }
    public int TimeSleep { get; set; } = 500;

    private const string PathConfig = "../config/appsettings.json";
    public static Config GetConfig()
    {
        try
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathConfig);
            var config =  File.ReadAllText(path);
            var cookiesConfig = JsonSerializer.Deserialize<Config>(config);

            if (string.IsNullOrEmpty(cookiesConfig.Cookies)
                || string.IsNullOrEmpty(cookiesConfig.BaseUrlBuyers) ||
                string.IsNullOrEmpty(cookiesConfig.BaseUrlSellers))
            {
                throw new Exception("Invalid config");
            }
            return cookiesConfig;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to load cookies.json: " + ex.Message);
        }
    }

    public void AddCookiesToReq(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("Cookie", Cookies);
    }
    
    public void AddHeaders(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:134.0) Gecko/20100101 Firefox/134.0");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        // httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
    }
}
