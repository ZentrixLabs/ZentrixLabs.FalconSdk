using System;

namespace ZentrixLabs.FalconSdk.Configuration;

/// <summary>
/// Configuration options for connecting to the CrowdStrike Falcon API.
/// </summary>
public class CrowdStrikeOptions
{
    /// <summary>
    /// The base URL of the CrowdStrike Falcon API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.crowdstrike.com";

    /// <summary>
    /// The client ID used for OAuth2 authentication.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The client secret used for OAuth2 authentication.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// The number of seconds to subtract from token expiration to ensure refresh before expiry.
    /// </summary>
    public int RefreshBufferSeconds { get; set; } = 60;

    /// <summary>
    /// The interval in seconds to wait before checking if the token should be refreshed early.
    /// </summary>
    public int EarlyRefreshSeconds { get; set; } = 10;

    /// <summary>
    /// Validates that required configuration fields are set correctly.
    /// Throws ArgumentException if any are missing or malformed.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
            throw new ArgumentException("BaseUrl must be provided.");

        if (!BaseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("BaseUrl must use HTTPS.");

        if (string.IsNullOrWhiteSpace(ClientId))
            throw new ArgumentException("ClientId must be provided.");

        if (string.IsNullOrWhiteSpace(ClientSecret))
            throw new ArgumentException("ClientSecret must be provided.");
    }
}
