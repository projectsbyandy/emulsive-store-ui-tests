using EmulsiveStoreE2E.Core.Helpers;
using EmulsiveStoreE2E.Core.UiComponents;
using EmulsiveStoreE2E.Ui.Services.Browser;
using EmulsiveStoreE2E.Ui.UiComponents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace EmulsiveStoreE2E.Ui.Ioc;

public static class ServiceExtensions
{
    public static IServiceCollection AddUiServices(this IServiceCollection services)
    {
        services.AddScoped<ILandingPage, LandingPage>();
        
        return services;
    }

    public static async Task<IServiceCollection> AddPlaywrightSupportAsync(this IServiceCollection services)
    {
        var playwright = await BrowserManager.CreatePlaywrightAsync();
        var browser = await BrowserManager.CreateBrowserAsync(ConfigManager.EnvConfig.BrowserConfig, playwright);
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        
        services.AddScoped<IPlaywright>(sp => playwright);
        services.AddScoped<IBrowser>(sp => browser);
        services.AddScoped<IBrowserContext>(sp => context);
        services.AddScoped<IPage>(sp => page);

        return services;
    }
}