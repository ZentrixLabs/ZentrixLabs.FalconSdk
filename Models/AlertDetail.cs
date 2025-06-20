using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Represents detailed information about a CrowdStrike Falcon alert.
/// </summary>
public class AlertDetail
{
    /// <summary>
    /// The unique identifier for the alert.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name or type of the alert.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The severity of the alert (e.g., low, medium, high, critical).
    /// </summary>
    [JsonPropertyName("severity")]
    public string? Severity { get; set; }

    /// <summary>
    /// The timestamp when the alert was created.
    /// </summary>
    [JsonPropertyName("created_timestamp")]
    public DateTimeOffset? CreatedTimestamp { get; set; }

    /// <summary>
    /// The status of the alert (e.g., new, in_progress, closed).
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// The host identifier (AID) associated with the alert.
    /// </summary>
    [JsonPropertyName("aid")]
    public string? Aid { get; set; }

    /// <summary>
    /// The host name associated with the alert.
    /// </summary>
    [JsonPropertyName("hostname")]
    public string? Hostname { get; set; }

    /// <summary>
    /// The description or summary of the alert.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Additional metadata or fields returned by the API.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalData { get; set; }
}