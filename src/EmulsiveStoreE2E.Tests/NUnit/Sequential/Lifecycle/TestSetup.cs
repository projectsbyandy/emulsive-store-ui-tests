using System.Diagnostics;
using EmulsiveStoreE2E.Core.Ioc;
using EmulsiveStoreE2E.Core.Services;
using EmulsiveStoreE2E.Ui.Ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Ardalis.GuardClauses;
using EmulsiveStoreE2E.Ui.Services;

namespace EmulsiveStoreE2E.Tests.NUnit.Sequential.Lifecycle;

internal class TestSetup
{
    protected ServiceProvider TestServices { get; private set; }
    private readonly Stopwatch _stopwatch = new();

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddConfigSupport()
            .AddUiServices()
            .AddLoggingSupport()
            .AddResilienceSupport();

        serviceCollection.AddScoped<IHeathCheck, HealthCheck>();
        
        await serviceCollection.AddPlaywrightSupportAsync();
        TestServices = serviceCollection.BuildServiceProvider();

        await Guard.Against.Null(TestServices.GetService<IHeathCheck>()).VerifyEmulsiveStoreRunningAsync();
        
        _stopwatch.Start();
    }
    
    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        var playwright = TestServices.GetRequiredService<IPlaywright>();
        var browser = TestServices.GetRequiredService<IBrowser>();
        var browserContext = TestServices.GetRequiredService<IBrowserContext>();
        var page = TestServices.GetRequiredService<IPage>();
        
        await browserContext.DisposeAsync();
        await browser.DisposeAsync();
        await page.CloseAsync();
        playwright.Dispose();
        
        await TestServices.DisposeAsync();
        _stopwatch.Stop();
        Console.WriteLine($"Elapsed time: {_stopwatch.ElapsedMilliseconds / 1000} sec");
    }

    private async Task PerformHealthCheck()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection
            .AddConfigSupport()
            .AddUiServices()
            .AddResilienceSupport();

        await serviceCollection.AddPlaywrightSupportAsync();
        TestServices = serviceCollection.BuildServiceProvider();

        await Guard.Against.Null(TestServices.GetService<IHeathCheck>()).VerifyEmulsiveStoreRunningAsync();
    }
}