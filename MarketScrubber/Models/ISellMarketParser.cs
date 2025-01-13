using System.Net.Http;
using System.Threading.Tasks;

namespace CSMarketBuff163SkinsParser;

public interface ISellMarketParser
{
    public ItemsRoot GetMostPopularItems(int count, HttpClient client);
}