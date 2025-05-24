using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Describes the structure of a Falcon device query response.
/// All fields are optional and may be null or missing depending on the API response.
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
    /// Use with caution — structure is loosely defined.
    /// </summary>
    [JsonPropertyName("errors")]
    public object? Errors { get; set; }

    /// <summary>
    /// Metadata associated with the query response.
    /// May contain pagination or query diagnostics.
    /// </summary>
    [JsonPropertyName("meta")]
    public object? Meta { get; set; }
}
