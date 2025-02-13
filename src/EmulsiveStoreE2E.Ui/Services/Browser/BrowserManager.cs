using EmulsiveStoreE2E.Core.Models.Config;
using Microsoft.Playwright;

namespace EmulsiveStoreE2E.Ui.Services.Browser;

public static class BrowserManager
{
    public static async Task<IPlaywright> CreatePlaywrightAsync() => await Playwright.CreateAsync();

    public static async Task<IBrowser> CreateBrowserAsync(BrowserConfig browserConfig, IPlaywright playwright)
    {
        var browserOptions = new BrowserTypeLaunchOptions() { Headless = browserConfig.IsHeadless };

        return browserConfig.BrowserInTest switch
        {
            BrowserInTest.Chrome => await playwright.Chromium.LaunchAsync(browserOptions),
            BrowserInTest.Firefox => await playwright.Firefox.LaunchAsync(browserOptions),
            BrowserInTest.WebKit => await playwright.Webkit.LaunchAsync(browserOptions),
            _ => throw new NotSupportedException(nameof(browserConfig.BrowserInTest))
        };
    }   
}
