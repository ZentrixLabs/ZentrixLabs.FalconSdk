namespace ZentrixLabs.FalconSdk.Congiguration;

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
}
