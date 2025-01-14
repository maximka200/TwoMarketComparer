using CSMarketBuff163SkinsParser;
using OfficeOpenXml;

namespace MarketScrubber.Services;

public class ExelWorker
{
    public static void CreateExcelFile(string filePath, List<Product> products)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        using (var package = new ExcelPackage())
        {
            var sheet = package.Workbook.Worksheets.Add("Sheet1");
            
            var headers = new[] { "Name", "Price Yuan", "Price Rub", "Сoefficient Benefit", "Buy Count", "Url Buy", "Url Sell" };
            
            for (var i = 0; i < headers.Length; i++)
            {
                sheet.Cells[1, i + 1].Value = headers[i];
            }
            
            for (var i = 2; i <= products.Count; i++)
            {
                var prod = products[i - 2];
                sheet.Cells[i, 1].Value = prod.Name;
                sheet.Cells[i, 2].Value = prod.PriceYuan;
                sheet.Cells[i, 3].Value = prod.PriceRub;
                sheet.Cells[i, 4].Value = prod.CoefficientBenefit;
                sheet.Cells[i, 5].Value = prod.Volume;
                sheet.Cells[i, 6].Value = "url";
                sheet.Cells[i, 7].Value = "url";
                sheet.Cells[i, 6].Hyperlink = new Uri(prod.UrlBuy);
                sheet.Cells[i, 7].Hyperlink = new Uri(prod.UrlSell);
            }
            
            FileInfo file = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath));
            package.SaveAs(file);
        }
    }
}