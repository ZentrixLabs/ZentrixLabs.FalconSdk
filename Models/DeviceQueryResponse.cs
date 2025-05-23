using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Represents the response from a CrowdStrike Falcon device query request.
/// </summary>
public class DeviceQueryResponse
{
    /// <summary>
    /// A list of device ID strings returned by the query.
    /// </summary>
    [JsonPropertyName("resources")]
    public List<string> Resources { get; set; } = [];

    /// <summary>
    /// Optional error information returned by the API.
    /// </summary>
    [JsonPropertyName("errors")]
    public List<object>? Errors { get; set; }

    /// <summary>
    /// Metadata associated with the query response.
    /// </summary>
    [JsonPropertyName("meta")]
    public Dictionary<string, object>? Meta { get; set; }
}
