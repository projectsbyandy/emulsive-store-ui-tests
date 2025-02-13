using Ardalis.GuardClauses;
using EmulsiveStoreE2E.Core.Models.Config;
using Microsoft.Extensions.Configuration;

namespace EmulsiveStoreE2E.Core.Helpers;

public class ConfigManager
{
    private static EnvironmentConfig? _environmentConfig;
    
    public static EnvironmentConfig EnvConfig
    {
        get => Guard.Against.Null(_environmentConfig, "Verify config has been setup first by calling AddConfigSupport");
        set => _environmentConfig = value;
    }

    public static IConfiguration GetConfiguration(string env)
    {
        var configuration =  new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        _environmentConfig = Guard.Against.Null(configuration.Get<EnvironmentConfig>());
        return configuration;
    }

    public static string GetEnvironmentInTestFromConfig()
    {
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("env.in.test.json");
        var config = configBuilder.Build();

        var envInTest = config.GetSection("envInTest").Get<string>();
        ArgumentException.ThrowIfNullOrEmpty(envInTest);

        return envInTest;
    }
}