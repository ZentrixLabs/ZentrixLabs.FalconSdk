using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Generic paginated response for Falcon API queries.
/// </summary>
public class PaginatedResponse<T>
{
    [JsonPropertyName("resources")]
    public List<T> Resources { get; set; } = [];

    [JsonPropertyName("meta")]
    public Meta? Meta { get; set; }

    [JsonPropertyName("errors")]
    public List<ApiError>? Errors { get; set; }
}

public class Meta
{
    [JsonPropertyName("pagination")]
    public Pagination? Pagination { get; set; }
}

public class Pagination
{
    [JsonPropertyName("next_token")]
    public string? NextToken { get; set; }

    [JsonPropertyName("total")]
    public int? Total { get; set; }
}