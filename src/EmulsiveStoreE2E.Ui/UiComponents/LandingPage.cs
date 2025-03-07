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

public class LandingPage(IPage page, EnvironmentConfig environmentConfig, IResilienceRetry resilienceRetry)
    : ILandingPage
{ 
    public async Task<(string, string)> GetIntroContentAsync()
    {
        var introElement = page.GetByTestId("IntroContent");
        var introHeading = Guard.Against.Null(await introElement.Locator("h1").TextContentAsync());
        var introText = Guard.Against.Null(await introElement.Locator("p").TextContentAsync());
        
        return (introHeading, introText);
    }

    public async Task<IList<FilmProduct>> ExtractFeaturedFilmsAsync(int numberToExtract)
    {
        if(environmentConfig.EnableDelayForParallelTest)
            Task.Delay(3000).Wait();

        ILocator? productElements = null;
        
        await resilienceRetry.PerformAsync(async () =>
        {
            var featureProducts = page.GetByTestId("FeaturedProducts");
            productElements = featureProducts.Locator("a[href^='/products']");
            var productCount = await productElements.CountAsync();
            
            if (productCount == numberToExtract)
                return;
            
            throw new RetryException("Expected number of featured items not present");
        }, TimeSpan.FromMilliseconds(200), 5);
        
        Guard.Against.Null(productElements);

        return await ProductExtraction.FromGridAsync(productElements);
    }

    public async Task SelectOurProductsAsync()
    {
        await page.GetByText("Our Products").ClickAsync();
    }
    
    public async Task SelectFeaturedProductAsync(string productName)
    {
        var parentElement = page.Locator($"img[alt='{productName}']").TraverseUpwards(3);
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