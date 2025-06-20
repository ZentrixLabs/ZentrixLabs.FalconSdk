using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Represents the top-level response envelope for alert details.
/// </summary>
public class AlertDetailEnvelope
{
    /// <summary>
    /// A list of detailed alert records returned by the API.
    /// </summary>
    [JsonPropertyName("resources")]
    public List<AlertDetail> Resources { get; set; } = [];

    /// <summary>
    /// A list of errors encountered during the API request, if any.
    /// </summary>
    [JsonPropertyName("errors")]
    public List<ApiError>? Errors { get; set; }
}