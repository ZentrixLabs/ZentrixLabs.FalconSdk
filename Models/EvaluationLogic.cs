using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Represents the evaluation logic for a vulnerability as reported by CrowdStrike Spotlight.
/// </summary>
public class EvaluationLogic
{
    /// <summary>
    /// The unique identifier for the evaluation logic.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name or title of the evaluation logic.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The description of the evaluation logic.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The logic or criteria used to evaluate the vulnerability.
    /// </summary>
    [JsonPropertyName("logic")]
    public string? Logic { get; set; }
}