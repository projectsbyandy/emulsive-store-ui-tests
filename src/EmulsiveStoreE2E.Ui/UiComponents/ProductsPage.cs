using System.Text.RegularExpressions;
using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.UiComponents;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace EmulsiveStoreE2E.Ui.UiComponents;

public class ProductsPage(IPage page) : WithProductFilter(page), IProductsPage
{
    private readonly ILocator _paginationLocator = page.GetByLabel("Pagination");
    private readonly IPage _page = page;

    public async Task<IList<FilmProduct>> GetProductsOnPageAsync(int pageNumber)
    {
        if(pageNumber > 1)
            await _paginationLocator.GetByRole(AriaRole.Link, new()
            {
                Name = pageNumber.ToString()
            }).ClickAsync();
        
        var productElements = _page.GetByTestId("products").Locator("a[href^='/products']");
       
        return await ProductExtraction.FromGridAsync(productElements);
    }

    public async Task ExpectMoreThanOnePageAsync()
    {
        await Expect(_paginationLocator).ToBeVisibleAsync();
    }
}