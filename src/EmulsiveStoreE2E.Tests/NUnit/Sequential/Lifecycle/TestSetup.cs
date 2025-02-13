using System.Diagnostics;
using EmulsiveStoreE2E.Core.Ioc;
using EmulsiveStoreE2E.Ui.Ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

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
            .AddResilienceSupport();

        await serviceCollection.AddPlaywrightSupportAsync();
        TestServices = serviceCollection.BuildServiceProvider();
        _stopwatch.Start();
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        var browser = TestServices.GetRequiredService<IBrowser>();
        await browser.CloseAsync();
        await TestServices.DisposeAsync();
        _stopwatch.Stop();
        Console.WriteLine($"Elapsed time: {_stopwatch.ElapsedMilliseconds / 1000} sec");
    }
}