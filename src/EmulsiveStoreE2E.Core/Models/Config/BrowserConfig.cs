using System.Text.Json.Serialization;

namespace EmulsiveStoreE2E.Core.Models.Config;

public class BrowserConfig
{
    [JsonPropertyName("browserInTest")]
    public BrowserInTest BrowserInTest { get; set; }

    [JsonPropertyName("isHeadless")]
    public bool IsHeadless { get; set; }
}