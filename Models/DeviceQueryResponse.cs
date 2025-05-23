using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

public class DeviceQueryResponse
{
    [JsonPropertyName("resources")]
    public List<string> Resources { get; set; } = [];

    [JsonPropertyName("errors")]
    public List<object>? Errors { get; set; }

    [JsonPropertyName("meta")]
    public Dictionary<string, object>? Meta { get; set; }
}
