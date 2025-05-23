using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

public class CrowdStrikeTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
