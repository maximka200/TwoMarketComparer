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
            
            var headers = new[] { "Name", "Price buff163 (yan)", "Price buff163 (rub)", "Price cs.go.market (rub)", "Coefficient benefit", "Buy count", "Url buy", "Url sell" };
            
            for (var i = 0; i < headers.Length; i++)
            {
                sheet.Cells[1, i + 1].Value = headers[i];
            }
            
            for (var i = 2; i <= products.Count; i++)
            {
                var prod = products[i - 2];
                sheet.Cells[i, 1].Value = prod.Name;
                sheet.Cells[i, 2].Value = prod.PriceYuanBuyer;
                sheet.Cells[i, 3].Value = prod.PriceRubBuyer;
                sheet.Cells[i, 4].Value = prod.PriceRubSeller;
                sheet.Cells[i, 5].Value = prod.CoefficientBenefit;
                sheet.Cells[i, 6].Value = prod.Volume;
                sheet.Cells[i, 7].Value = "click here";
                sheet.Cells[i, 8].Value = "click here";
                sheet.Cells[i, 7].Hyperlink = new Uri(prod.UrlBuy);
                sheet.Cells[i, 8].Hyperlink = new Uri(prod.UrlSell);
            }
            
            FileInfo file = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath));
            package.SaveAs(file);
        }
    }
}