using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using EmulsiveStoreE2E.Core.Exceptions;
using EmulsiveStoreE2E.Core.Helpers.RetryHelper;
using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.Models.Config;
using EmulsiveStoreE2E.Core.UiComponents;
using EmulsiveStoreE2E.Ui.Services;
using Microsoft.Playwright;

namespace EmulsiveStoreE2E.Ui.UiComponents;

public class LandingPage : ILandingPage
{
    private readonly IPage _page;
    private readonly EnvironmentConfig _environmentConfig;
    private readonly IResilienceRetry _resilienceRetry;
    
    public LandingPage(IPage page, EnvironmentConfig environmentConfig, IResilienceRetry resilienceRetry)
    {
        _page = page;
        _environmentConfig = environmentConfig;
        _resilienceRetry = resilienceRetry;
    }

    public async Task NavigateToStoreAsync()
    {
        await _page.GotoAsync(_environmentConfig.EmulsiveStoreUrl);
    }

    public async Task<(string, string)> GetIntroContentAsync()
    {
        var introElement = _page.GetByTestId("IntroContent");
        var introHeading = Guard.Against.Null(await introElement.Locator("h1").TextContentAsync());
        var introText = Guard.Against.Null(await introElement.Locator("p").TextContentAsync());
        
        return (introHeading, introText);
    }

    public async Task<IList<FilmProduct>> ExtractFeaturedFilmsAsync(int numberToExtract)
    {
        if(_environmentConfig.EnableDelayForParallelTest)
            Task.Delay(3000).Wait();

        ILocator? productElements = null;
        await _resilienceRetry.PerformAsync(async () =>
        {
            var featureProducts = _page.GetByTestId("FeaturedProducts");
            productElements = featureProducts.Locator("a[href^='/products']");
            var productCount = await productElements.CountAsync();
            
            if(productCount == numberToExtract)
                return;
            
            throw new RetryException("Expected number of featured items not present");
        }, TimeSpan.FromMilliseconds(200), 5);
        
        Guard.Against.Null(productElements);
        var productCount = await productElements.CountAsync();
        var products = new List<FilmProduct>();

        for (var i = 0; i < productCount; i++)
        {
            await _resilienceRetry.UntilTrueAsync("Wait for images to load", async () => await productElements.Nth(i).Locator("div img").IsVisibleAsync(), TimeSpan.FromMilliseconds(200), _environmentConfig.RetryConfig.DefaultRetries); 
            products.Add(new FilmProduct()
            {
                Id = SelectProductIdFromUrl(Guard.Against.Null(await productElements.Nth(i).GetAttributeAsync("href"))),
                ImageUrl = Guard.Against.Null(await productElements.Nth(i).Locator("div img").GetAttributeAsync("src")),
                Name = Guard.Against.Null(await productElements.Nth(i).Locator("h2").TextContentAsync()),
                PriceWithCurrency = Guard.Against.Null(await productElements.Nth(i).Locator("div p").TextContentAsync())
            });
        }
        
        return products;
    }

    public async Task SelectOurProductsAsync()
    {
        await _page.GetByText("Our Products").ClickAsync();
    }
    
    public async Task SelectFeaturedProductsAsync(string productName)
    {
        var parentElement = _page.Locator($"img[alt='{productName}']").TraverseUpwards(3);
        await parentElement.ClickAsync();
    }

    private int SelectProductIdFromUrl(string urlPart)
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