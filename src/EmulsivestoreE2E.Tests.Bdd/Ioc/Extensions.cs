using Autofac;
using EmulsiveStoreE2E.Core.Helpers;
using EmulsiveStoreE2E.Core.Helpers.RetryHelper;
using EmulsiveStoreE2E.Core.Models.Config;
using EmulsiveStoreE2E.Core.UiComponents;
using EmulsiveStoreE2E.Ui.Services.Browser;
using EmulsiveStoreE2E.Ui.UiComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Serilog;

namespace EmulsiveStoreE2E.Tests.Bdd.Ioc;

internal static class Extensions
{
    public static ContainerBuilder AddConfigSupport(this ContainerBuilder builder)
    {
        var environmentInTest = Environment.GetEnvironmentVariable("ENVIRONMENTINTEST") ??
                                ConfigManager.GetEnvironmentInTestFromConfig();

        var configuration = ConfigManager.GetConfiguration(environmentInTest.ToLower());

        ArgumentException.ThrowIfNullOrEmpty(environmentInTest);

        var environmentConfig = configuration.Get<EnvironmentConfig>();
        ArgumentNullException.ThrowIfNull(environmentConfig);

        builder.RegisterInstance(environmentConfig).SingleInstance();

        return builder;
    }

    public static ContainerBuilder AddResilienceSupport(this ContainerBuilder builder)
    {
        builder.RegisterType<ResilienceRetry>().As<IResilienceRetry>().SingleInstance();
        builder.RegisterInstance(new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger()).As<ILogger>().SingleInstance();

        return builder;
    }

    public static ContainerBuilder AddUiServices(this ContainerBuilder builder)
    {
        builder.RegisterType<LandingPage>().As<ILandingPage>().SingleInstance();
        builder.RegisterType<WithProductFilter>().As<IWithProductFilter>().SingleInstance();
        builder.RegisterType<ProductsPage>().As<IProductsPage>().SingleInstance();
        builder.RegisterType<Navigate>().As<INavigate>().SingleInstance();

        return builder;
    }

    public static ContainerBuilder AddPlaywrightComponents(this ContainerBuilder builder)
    {
        var playwright = (BrowserManager.CreatePlaywrightAsync()).Result;
        var browser = (BrowserManager.CreateBrowserAsync(ConfigManager.EnvConfig.BrowserConfig, playwright)).Result;
        var context = (browser.NewContextAsync()).Result;
        var page = (context.NewPageAsync()).Result;

        builder.RegisterInstance(playwright).As<IPlaywright>().SingleInstance();
        builder.RegisterInstance(browser).As<IBrowser>().SingleInstance();
        builder.RegisterInstance(context).As<IBrowserContext>().SingleInstance();
        builder.RegisterInstance(page).As<IPage>().SingleInstance();

        return builder;
    }
}