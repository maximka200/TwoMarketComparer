using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using buff163;
using CSMarketBuff163SkinsParser;

namespace Buff163ItemSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var conf = CookiesConfig.GetConfig();
                HttpClient httpClient = new HttpClient();
                conf.AddHeaders(httpClient);
                conf.AddCookiesToReq(httpClient);
                var csmark = new Buff163Parser(conf);
                var buyer = new CSMarketParser(conf);
                
                Console.WriteLine("Enter min volume:");
                var minVolume = int.Parse(Console.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
                Console.WriteLine("Enter Yuan to Rub:");
                var yanToRub = float.Parse(Console.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
                
                var merged = new MergeBuyerSeller(csmark, buyer);
                var products = merged.GetMergedItems(minVolume, httpClient, yanToRub);
                
                ExelWorker.CreateExcelFile("./test.xlsx", products);
            } 
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
