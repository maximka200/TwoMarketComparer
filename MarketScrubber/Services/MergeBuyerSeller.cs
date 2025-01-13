using System.Globalization;
using buff163;

namespace CSMarketBuff163SkinsParser;

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
    
    public List<Product> GetMergedItems(int volume, HttpClient client, float yuanToRub, CookiesConfig config)
    {
        try
        {
            var items = GetMostPopularItems(volume, client, config.BaseUrlSellers);
            Console.WriteLine("Success parsing data from seller");
            var productList = new List<Product>();
            foreach (var item in items.Items)
            {
                var name = item.MarketHashName;
                Thread.Sleep(500);
                var buyerItem = GetItemsByNameFromBuyer(name, client, config.BaseUrlBuyers);
                if (buyerItem == null || item.Price == null)
                {
                    Console.WriteLine($"Unfortunatelly, we can't find price on {name}");
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