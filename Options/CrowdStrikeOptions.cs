namespace ZentrixLabs.FalconSdk.Options;

public class CrowdStrikeOptions
{
    public string BaseUrl { get; set; } = "https://api.crowdstrike.com";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
