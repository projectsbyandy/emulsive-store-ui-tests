using System.Text.RegularExpressions;
using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.Models.Config;
using EmulsiveStoreE2E.Core.UiComponents;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace EmulsiveStoreE2E.Ui.UiComponents;

public class ProductsPage(IPage page, EnvironmentConfig environmentConfig) : WithProductFilter(page, environmentConfig), IProductsPage
{
    private readonly ILocator _paginationLocator = page.GetByLabel("Pagination");
    private readonly ILocator _productElements = page.GetByTestId("products").Locator("a[href^='/products']");
    private readonly ILocator _productDetails = page.GetByTestId("productDetails");
    
    private readonly IPage _page = page;
    private readonly EnvironmentConfig _environmentConfig = environmentConfig;

    public async Task<Dictionary<string, bool>> IsKeywordPresentInAllProductAsync(string keyword)
    {
        var filterOutcome = new Dictionary<string, bool>();
        
        var products = await GetAllProductsAsync();
        
        foreach (var filmProduct in products)
        {
            if (filmProduct.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) 
            {
                filterOutcome.Add(filmProduct.Name, true);   
            }
            else
            {
                await _page.GotoAsync(_environmentConfig.EmulsiveStoreUrl + filmProduct.DetailsUrlPart);
                var product = await ProductExtraction.FromDetailsAsync(_productDetails);
                filterOutcome.Add(filmProduct.Name, product.Manufacturer!.Contains(keyword, StringComparison.OrdinalIgnoreCase) || product.Description!.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }
        }
        
        return filterOutcome;
    }
    
    private async Task<IList<FilmProduct>> GetAllProductsAsync()
    {
        var products = new List<FilmProduct>();

        var links = _paginationLocator.GetByRole(AriaRole.Link, new()
        {
            NameRegex = new Regex(@"^\d+$")
        });

        var linkCount = await links.CountAsync();

        if (linkCount is 0)
            return await ProductExtraction.FromGridAsync(_productElements);
        
        for (var i = 0; i < linkCount; i++)
        {
            await links.Nth(i).ClickAsync();
        
            await Expect(_page.GetByTestId("products").Locator("a[href^='/products']").Last).ToBeVisibleAsync();
            var productsOnPage = await ProductExtraction.FromGridAsync(_productElements);
            products.AddRange(productsOnPage);
        }

        return products;
    }
}