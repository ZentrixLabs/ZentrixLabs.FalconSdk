using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Represents the response payload returned by the CrowdStrike Falcon token endpoint.
/// </summary>
public class CrowdStrikeTokenResponse
{
    /// <summary>
    /// The bearer token used for subsequent Falcon API requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The duration in seconds until the token expires.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
