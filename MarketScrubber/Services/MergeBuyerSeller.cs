using System.Globalization;

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

    private Buyer GetItemsByNameFromBuyer(string itemName, HttpClient client)
    {
        return buyer.GetItemByName(itemName, client);
    }

    private ItemsRoot GetMostPopularItems(int count, HttpClient client)
    {
        return seller.GetMostPopularItems(count, client);
    }
    
    public List<Product> GetMergedItems(int volume, HttpClient client, float yuanToRub)
    {
        var items = GetMostPopularItems(volume, client);
        Console.WriteLine("Success parsing data from seller");
        var productList = new List<Product>();
        foreach (var item in items.Items)
        {
            Thread.Sleep(500);
            var buyerItem = GetItemsByNameFromBuyer(item.MarketHashName, client);
            if (buyerItem == null || buyerItem.Price == null || item.Price == null)
            {
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
    
    public static string ChangeDoteOnComme(string price)
    {
        return price.Replace(".", ",");
    }
    
    private string TryParsePrice(string price)
    {
        if (float.TryParse(price, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedPrice))
        {
            return (parsedPrice).ToString(new CultureInfo("ru-RU"));
        }
        return "0"; 
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