using System.Globalization;
using System.Web;
using CSMarketBuff163SkinsParser;

namespace MarketScrubber.Services;

public class MergeBuyerSeller
{
    public MergeBuyerSeller(IBuyMarketParser buyer, ISellMarketParser seller)
    {
        this.buyer = buyer;
        this.seller = seller;
    }
    
    private const string SearchUrlSeller = "https://market.csgo.com/ru/?search=";
    private const string SearchUrlBuyer = "https://buff.163.com/market/csgo#game=csgo&page_num=1&tab=selling&search=";
    private IBuyMarketParser buyer;
    private ISellMarketParser seller;
    
    private async Task<Buyer?> GetItemsByNameFromBuyer(string itemName, HttpClient client, string baseUrl)
    {
        return await buyer.GetItemByNameAsync(itemName, client, baseUrl);
    }

    private ItemsRoot GetMostPopularItems(int volume, float price, HttpClient client, string baseUrl)
    {
        return seller.GetMostPopularItems(volume, price, client, baseUrl);
    }
    
    public async Task<List<Product>> GetMergedItemsAsync(int volume, float price, HttpClient client, float yuanToRub, Config config)
    {
        var op = nameof(GetMergedItemsAsync);
        try
        {
            var items = GetMostPopularItems(volume, price, client, config.BaseUrlSellers);
            var count = items.Items.Count();
            if (count == 0)
            {
                throw new Exception("No items found from seller");
            }
            var productList = new List<Product>();
            var counter = 1; // for progress bar
            foreach (var item in items.Items)
            {
                var name = item.MarketHashName;
                Thread.Sleep(TimeSpan.FromMilliseconds(config.TimeSleep * new Random().NextDouble() + config.TimeSleep * 0.9));
                var buyerItem = await GetItemsByNameFromBuyer(name, client, config.BaseUrlBuyers);
                ConsoleWriter.SimulateLoadingBarV2(count, counter);
                counter++;
                if (buyerItem == null || item.Price == null)
                {
                    // Console.WriteLine($"Unfortunatelly, we can't find price on {name}");
                    continue;
                }

                var sellerUrl = $"{SearchUrlSeller}{item.MarketHashName}";
                var buyerUrl = $"{SearchUrlBuyer}{item.MarketHashName}";

                productList.Add(new Product
                {
                    Name = item.MarketHashName,
                    PriceYuanBuyer = ChangeDoteOnComme(buyerItem.Price),
                    PriceRubBuyer = ChangeDoteOnComme(TryParsePriceToRub(buyerItem.Price, yuanToRub)),
                    PriceRubSeller = ChangeDoteOnComme(item.Price),
                    CoefficientBenefit = TryParseCoefficient(buyerItem.Price, item.Price, yuanToRub),
                    Volume = item.Volume,
                    UrlBuy = buyerUrl,
                    UrlSell = sellerUrl
                });
            }

            return productList;
        }
        catch (Exception ex)
        {
            throw new Exception($"{op}: {ex.Message}", ex);
        }
    }
    
    private static string TryParsePriceToRub(string price, float yuanToRub)
    {
        if (float.TryParse(price, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedPrice))
        {
            return (parsedPrice * yuanToRub).ToString(CultureInfo.InvariantCulture);
        }
        return "0";
    }
    
    public static string ChangeDoteOnComme(string price)
    {
        return price.Replace(".", ",");
    }

    private string TryParseCoefficient(string buyerPrice, string sellerPrice, float yuanToRub)
    {
        if (float.TryParse(buyerPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedBuyerPrice) &&
            float.TryParse(sellerPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedSellerPrice))
        {
            if (parsedBuyerPrice == 0 || parsedSellerPrice == 0)
            {
                return "0";
            }
            return (parsedSellerPrice / (yuanToRub * parsedBuyerPrice)).ToString(new CultureInfo("ru-RU"));
        }
        return "0"; 
    }
}