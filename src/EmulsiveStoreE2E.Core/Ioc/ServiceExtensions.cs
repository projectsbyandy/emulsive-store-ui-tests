using EmulsiveStoreE2E.Core.Helpers;
using EmulsiveStoreE2E.Core.Helpers.RetryHelper;
using EmulsiveStoreE2E.Core.Models.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EmulsiveStoreE2E.Core.Ioc;

public static class ServiceExtensions
{
    public static IServiceCollection AddConfigSupport(this IServiceCollection serviceCollection)
    {
        var environmentInTest = Environment.GetEnvironmentVariable("ENVIRONMENTINTEST") ??
                                ConfigManager.GetEnvironmentInTestFromConfig();
        
        var configuration = ConfigManager.GetConfiguration(environmentInTest.ToLower());

        ArgumentException.ThrowIfNullOrEmpty(environmentInTest);
        
        var environmentConfig = configuration.Get<EnvironmentConfig>();
        ArgumentNullException.ThrowIfNull(environmentConfig);
        serviceCollection.AddScoped(sp => environmentConfig);

        return serviceCollection;
    }

    public static IServiceCollection AddResilienceSupport(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IResilienceRetry, ResilienceRetry>();
        serviceCollection.AddScoped<ILogger>(sp => new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger());
        return serviceCollection;
    }
}