using System.Collections.Concurrent;
using System.Diagnostics;
using Ardalis.GuardClauses;
using EmulsiveStoreE2E.Core.Ioc;
using EmulsiveStoreE2E.Ui.Ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using NUnit.Framework.Interfaces;

namespace EmulsiveStoreE2E.Tests.NUnit.Parallel.Lifecycle;

internal class TestLifeCycle : IDisposable
{
    private static readonly ConcurrentDictionary<string, ServiceProvider> ServiceProviders = new();
    private static readonly Stopwatch Stopwatch = new();
    
    protected T GetService<T>()
    {
        var serviceProvider = Guard.Against.Null(GetServiceProvider(TestContext.CurrentContext.Test.ID));
        Console.WriteLine($"Retrieving {typeof(T).Name} for Test: {TestContext.CurrentContext.Test.ID}");
        
        var service =  Guard.Against.Null(serviceProvider.GetService<T>(), nameof(T),
            "Unable to retrieve from service provider");
        
        Console.WriteLine($"Service hash {service?.GetHashCode()}");
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
            var page = GetServiceProvider(TestContext.CurrentContext.Test.ID).GetService<IPage>();
            
            Guard.Against.Null(page);
            await page.ScreenshotAsync(new()
            {
                Path = $"./screenshots/{DateTime.Now}-{TestContext.CurrentContext.Test.ID}.png",
                FullPage = true,
            });
            Console.WriteLine($"Written screenshot failureScreenshot-{DateTime.Now}-{TestContext.CurrentContext.Test.ID}.png");
        }
        
        GetServiceProvider(TestContext.CurrentContext.Test.ID).Dispose();
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
}