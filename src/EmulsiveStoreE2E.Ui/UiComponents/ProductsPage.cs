using EmulsiveStoreE2E.Core.UiComponents;
using Microsoft.Playwright;

namespace EmulsiveStoreE2E.Ui.UiComponents;

public class ProductsPage(IPage page) : WithProductFilter(page), IProductsPage
{
    
}