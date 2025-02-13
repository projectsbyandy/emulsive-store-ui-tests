using System.Text.Json.Serialization;

namespace EmulsiveStoreE2E.Core.Models.Config;

public record RetryConfig
{
    [JsonPropertyName("defaultRetries")]
    public int DefaultRetries { get; set; }
}