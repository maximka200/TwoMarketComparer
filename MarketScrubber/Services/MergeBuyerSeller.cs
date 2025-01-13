using System.Globalization;
using CSMarketBuff163SkinsParser;

namespace MarketScrubber.Services;

public class MergeBuyerSeller
{
    private IBuyMarketParser buyer;
    private ISellMarketParser seller;
    
    public MergeBuyerSeller(IBuyMarketParser buyer, ISellMarketParser seller)
    {
        this.buyer = buyer;
        this.seller = seller;
    }

    private Buyer? GetItemsByNameFromBuyer(string itemName, HttpClient client, string baseUrl)
    {
        return buyer.GetItemByName(itemName, client, baseUrl);
    }

    private ItemsRoot GetMostPopularItems(int count, HttpClient client, string baseUrl)
    {
        return seller.GetMostPopularItems(count, client, baseUrl);
    }
    
    public List<Product> GetMergedItems(int volume, HttpClient client, float yuanToRub, Config config)
    {
        try
        {
            var items = GetMostPopularItems(volume, client, config.BaseUrlSellers);
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
                Thread.Sleep(config.TimeSleep);
                var buyerItem = GetItemsByNameFromBuyer(name, client, config.BaseUrlBuyers);
                ConsoleWriter.SimulateLoadingBarV2(count, counter);
                counter++;
                if (buyerItem == null || item.Price == null)
                {
                    // Console.WriteLine($"Unfortunatelly, we can't find price on {name}");
                    continue;
                }

                productList.Add(new Product
                {
                    Name = item.MarketHashName,
                    PriceYuan = ChangeDoteOnComme(buyerItem.Price),
                    PriceRub = ChangeDoteOnComme(item.Price),
                    CoefficientBenefit = TryParseCoefficient(buyerItem.Price, item.Price, yuanToRub),
                    Volume = item.Volume,
                    UrlBuy = buyerItem.UrlBuy,
                    UrlSell = item.UrlSell
                });
            }

            return productList;
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex.Message}", ex);
        }
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
            return (parsedSellerPrice / (yuanToRub * parsedBuyerPrice)).ToString(new CultureInfo("ru-RU"));
        }
        return "0"; 
    }
}