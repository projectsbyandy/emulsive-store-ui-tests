using System.Text.Json.Serialization;

namespace EmulsiveStoreE2E.Core.Models.Config;

public record EnvironmentConfig
{
    [JsonPropertyName("emulsiveStoreUrl")]
    public required string EmulsiveStoreUrl  { get; init; }
    [JsonPropertyName("browserConfig")]
    public required BrowserConfig BrowserConfig  { get; init; }
    [JsonPropertyName("retryConfig")]
    public required RetryConfig RetryConfig { get; set; }
    [JsonPropertyName("enableDelayForParallelTest")]
    public bool EnableDelayForParallelTest { get; set; }
}