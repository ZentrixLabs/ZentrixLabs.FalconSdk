using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Represents pagination information.
/// </summary>
public class PaginationInfo
{
    [JsonPropertyName("next_token")]
    public string? NextToken { get; set; }
}