 using EmulsiveStoreE2E.Core.Models.Config;
 using EmulsiveStoreE2E.Core.Services;
 using Microsoft.Playwright;
 using Serilog;
 using static Microsoft.Playwright.Assertions;

namespace EmulsiveStoreE2E.Ui.Services;

public class HealthCheck(IPage page, EnvironmentConfig config, ILogger logger) : IHeathCheck
{
    public async Task VerifyEmulsiveStoreRunningAsync()
    {
        try
        {
            await page.GotoAsync(config.EmulsiveStoreUrl);
            await Expect(page.GetByTestId("IntroContent")).ToBeVisibleAsync();
        }
        catch (Exception ex)
        {
            logger.Error("Health check failed due to {@Error}", ex.Message);
            throw;
        }
    }
}