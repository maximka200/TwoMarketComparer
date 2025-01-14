﻿using CSMarketBuff163SkinsParser;
using MarketScrubber.Services;

namespace Buff163ItemSearch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Blue;
                
                var conf = Config.GetConfig();
                HttpClient httpClient = new HttpClient();
                conf.AddHeaders(httpClient);
                conf.AddCookiesToReq(httpClient);
                var csmark = new Buff163Parser();
                var buyer = new CSMarketParser();

                Console.WriteLine("Enter min volume (number of purchases per week):");
                var minVolume = int.Parse(Console.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
                Console.WriteLine("Enter min price (rub):");
                var minPrice = float.Parse(Console.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
                Console.WriteLine("Enter yuan to rub:");
                var yanToRub = float.Parse(Console.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);

                var merged = new MergeBuyerSeller(csmark, buyer);
                var products = await merged.GetMergedItemsAsync(minVolume, minPrice, httpClient, yanToRub, conf);

                ExelWorker.CreateExcelFile("./test.xlsx", products);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\rError: " + ex.Message);
            }
        }
        
    }
}
