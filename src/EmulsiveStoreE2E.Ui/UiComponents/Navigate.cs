using EmulsiveStoreE2E.Core.Helpers;
using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.Models.Config;
using EmulsiveStoreE2E.Core.UiComponents;
using Microsoft.Playwright;

namespace EmulsiveStoreE2E.Ui.UiComponents;

public class Navigate(IPage page, EnvironmentConfig environmentConfig) : INavigate
{
    public async Task To(StoreSection storeSection)
    {
        var url = string.Empty;
        
        switch (storeSection)
        {
            case StoreSection.Landing:
                url = environmentConfig.EmulsiveStoreUrl;
                break;
            case StoreSection.Products:
                url = environmentConfig.EmulsiveStoreUrl + storeSection.GetDescription();
                break;
        }
        
        await page.GotoAsync(url);
    }
}