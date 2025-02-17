using Microsoft.Playwright;
using Reqnroll;

namespace EmulsiveStoreE2E.Tests.Bdd.Hooks;

[Binding]
internal class TearDownHooks(IPage page, IBrowser browser, IBrowserContext browserContext, IPlaywright playwright)
{
    [AfterScenario]
    public async Task AfterScenario()
    {
        await page.CloseAsync();
        await browserContext.DisposeAsync();
        await browser.DisposeAsync();
        playwright.Dispose();
    }
}