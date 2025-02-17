using System.Collections.Concurrent;
using System.Diagnostics;
using Ardalis.GuardClauses;
using EmulsiveStoreE2E.Core.Ioc;
using EmulsiveStoreE2E.Ui.Ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.VisualBasic;
using NUnit.Framework.Interfaces;
using Serilog;

namespace EmulsiveStoreE2E.Tests.NUnit.Parallel.Lifecycle;

internal class TestLifeCycle : IDisposable
{
    private static readonly ConcurrentDictionary<string, ServiceProvider> ServiceProviders = new();
    private static readonly Stopwatch Stopwatch = new();

    protected T GetService<T>()
    {
        var logger = GetLogger();
        var serviceProvider = Guard.Against.Null(GetServiceProvider(TestContext.CurrentContext.Test.ID));
        logger.Information("Retrieving {@ServiceName} for Test: {@TestId}", typeof(T).Name,TestContext.CurrentContext.Test.ID );
        
        var service =  Guard.Against.Null(serviceProvider.GetService<T>(), nameof(T),
            "Unable to retrieve from service provider");
        
        logger.Information("Service hash: {@Hash}", service?.GetHashCode());
        
        return service;
    }

    [OneTimeSetUp]
    public static void SetUp()
    {
        Stopwatch.Start();
    }
    
    private ServiceProvider GetServiceProvider(string testId)
    {
        ServiceProviders.TryGetValue(testId, out var result);
        
        return Guard.Against.Null(result);
    }
    
    protected async Task SetupServiceProviderAsync()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddConfigSupport()
            .AddUiServices()
            .AddResilienceSupport();

        await serviceCollection.AddPlaywrightSupportAsync();

        var serviceProvider= serviceCollection.BuildServiceProvider();
        
        ServiceProviders.TryAdd(TestContext.CurrentContext.Test.ID, serviceProvider);
    }

    [TearDown]
    public async Task TearDownScope()
    {
        if (TestContext.CurrentContext.Result.Outcome == ResultState.Failure)
        {
            var page = GetService<IPage>();
            Guard.Against.Null(page);
            await page.ScreenshotAsync(new()
            {
                Path = $"./screenshots/{DateTime.Now}-{TestContext.CurrentContext.Test.ID}.png",
                FullPage = true,
            });
            
            GetLogger().Information("Written screenshot failureScreenshot-{@DateTime}-{@TestId}.png", DateTime.Now, TestContext.CurrentContext.Test.ID);
        }

        await DisposeOfPlaywrightAsync();
        await GetServiceProvider(TestContext.CurrentContext.Test.ID).DisposeAsync();

        if (ServiceProviders.TryRemove(TestContext.CurrentContext.Test.ID, out _) is false)
        {
            throw new Exception($"Unable to remove service provider from dictionary : {TestContext.CurrentContext.Test.ID}");
        }
    }

    private async Task DisposeOfPlaywrightAsync()
    {
        var playwrightService = Guard.Against.Null(GetService<IPlaywright>());
        var browserService = Guard.Against.Null(GetService<IBrowser>());
        var browserContextService = Guard.Against.Null(GetService<IBrowserContext>());
        var pageService = Guard.Against.Null(GetService<IPage>());

        var logger = GetLogger();
        logger.Information("Disposing of pageService: {@HashCode}", pageService.GetHashCode());
        await pageService.CloseAsync();
        logger.Information("Disposed of pageService");
        
        logger.Information("Disposing of browserContext: {@HashCode}", browserContextService.GetHashCode());
        await browserContextService.DisposeAsync();
        logger.Information("Disposed of browserContext");
        
        logger.Information("Disposing of browserService: {@HashCode}", browserService.GetHashCode());
        await browserService.DisposeAsync();
        logger.Information("Disposed of browserService");
        
        logger.Information("Disposing of playwrightService: {@HashCode}", playwrightService.GetHashCode());
        playwrightService.Dispose();
        logger.Information("Disposed of playwrightService");
    }

    [OneTimeTearDown]
    public static void TearDown()
    {
        ServiceProviders.Clear();
        Stopwatch.Stop();
        Console.WriteLine($"Elapsed time: {Stopwatch.ElapsedMilliseconds / 1000} sec");
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);    
    }

    private ILogger GetLogger()
        => Guard.Against.Null(GetServiceProvider(TestContext.CurrentContext.Test.ID).GetService<ILogger>());
}