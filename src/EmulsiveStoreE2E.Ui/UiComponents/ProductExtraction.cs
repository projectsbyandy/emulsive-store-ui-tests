using System.Text.RegularExpressions;
using EmulsiveStoreE2E.Core.Models;
using Microsoft.Playwright;
using Ardalis.GuardClauses;
using static Microsoft.Playwright.Assertions;

namespace EmulsiveStoreE2E.Ui.UiComponents;

internal class ProductExtraction
{
    public static async Task<IList<FilmProduct>> FromGridAsync(ILocator source)
    {
        var productCount = await source.CountAsync();
        var products = new List<FilmProduct>();

        for (var i = 0; i < productCount; i++)
        {
            await Expect(source.Nth(i).Locator("div img")).ToBeVisibleAsync();
            products.Add(new FilmProduct()
            {
                Id = SelectProductIdFromUrl(Guard.Against.Null(await source.Nth(i).GetAttributeAsync("href"))),
                ImageUrl = Guard.Against.Null(await source.Nth(i).Locator("div img").GetAttributeAsync("src")),
                Name = Guard.Against.Null(await source.Nth(i).Locator("h2").TextContentAsync()),
                PriceWithCurrency = Guard.Against.Null(await source.Nth(i).Locator("div p").TextContentAsync())
            });
        }
        
        return products;
    }
    
    private static int SelectProductIdFromUrl(string urlPart)
    {
        var pattern = @"products/(\d+)";

        var match = Regex.Match(urlPart, pattern);

        if (match.Success)
        {
            var numberString = match.Groups[1].Value;
            return int.Parse(numberString);
        }
        
        throw new ArgumentException($"No match found for {urlPart}");
    }
}