using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Represents a remediation for a vulnerability as reported by CrowdStrike Spotlight.
/// </summary>
public class VulnerabilityRemediation
{
    /// <summary>
    /// The unique identifier for the remediation.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name or title of the remediation.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The description of the remediation.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The list of vulnerability IDs this remediation addresses.
    /// </summary>
    [JsonPropertyName("vulnerability_ids")]
    public List<string>? VulnerabilityIds { get; set; }

    /// <summary>
    /// The remediation steps or instructions.
    /// </summary>
    [JsonPropertyName("remediation")]
    public string? Remediation { get; set; }

    /// <summary>
    /// The severity of the vulnerabilities addressed.
    /// </summary>
    [JsonPropertyName("severity")]
    public string? Severity { get; set; }
}