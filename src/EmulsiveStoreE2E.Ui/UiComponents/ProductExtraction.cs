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
                Id = SelectProductIdFromUrl(Guard.Against.NullOrEmpty(await source.Nth(i).GetAttributeAsync("href"))),
                ImageUrl = Guard.Against.NullOrEmpty(await source.Nth(i).Locator("div img").GetAttributeAsync("src")),
                DetailsUrlPart = Guard.Against.NullOrEmpty(await source.Nth(i).GetAttributeAsync("href")),
                Name = Guard.Against.NullOrEmpty(await source.Nth(i).Locator("h2").TextContentAsync()),
                PriceWithCurrency = Guard.Against.NullOrEmpty(await source.Nth(i).Locator("div p").TextContentAsync())
            });
        }
        
        return products;
    }
    
    public static async Task<FilmProduct> FromDetailsAsync(ILocator source)
    {
        return new FilmProduct()
        {
            Name = Guard.Against.NullOrEmpty(await source.GetByTestId("name").TextContentAsync()),
            ImageUrl = Guard.Against.NullOrEmpty(await source.GetByTestId("imageUrl").GetAttributeAsync("src")),
            Manufacturer = Guard.Against.NullOrEmpty(await source.GetByTestId("manufacturer").TextContentAsync()),
            Iso = ExtractValues(Guard.Against.NullOrEmpty(await source.GetByTestId("iso").TextContentAsync()), "ISO:"),
            Format = ExtractValues(Guard.Against.NullOrEmpty(await source.GetByTestId("format").TextContentAsync()), "Format:"),
            Description = Guard.Against.NullOrEmpty(await source.GetByTestId("description").TextContentAsync()),
            PriceWithCurrency = Guard.Against.NullOrEmpty(await source.GetByTestId("price").TextContentAsync()),
        };
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

    private static string? ExtractValues(string rawText, string propertyName)
    {
        var found = Regex.Match(rawText, @$"{propertyName}\s*(.*)");
        return found.Success ? found.Groups[1].Value : null;
    }
}