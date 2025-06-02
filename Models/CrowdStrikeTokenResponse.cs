using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Represents the response payload returned by the CrowdStrike Falcon token endpoint.
/// Includes bearer token and expiration metadata. Values may be missing in case of API drift or error.
/// </summary>
public class CrowdStrikeTokenResponse
{
    /// <summary>
    /// The bearer token used for subsequent Falcon API requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The duration in seconds until the token expires. May be zero or omitted in edge cases.
    /// Consumers should apply a fallback if this value is less than or equal to zero.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Optional error details returned if the token request failed.
    /// </summary>
    [JsonPropertyName("errors")]
    public List<ApiError>? Errors { get; set; }
}
