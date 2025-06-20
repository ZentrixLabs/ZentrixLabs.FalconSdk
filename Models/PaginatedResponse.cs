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


/// <summary>
/// Represents metadata for a paginated response.
/// Contains pagination information such as next token and total items.
/// </summary>
public class Meta
{
    [JsonPropertyName("pagination")]
    public Pagination? Pagination { get; set; }
}

/// <summary>
/// Represents pagination information in the API response.
/// Contains the next token for fetching additional pages and the total number of items.
/// </summary>
public class Pagination
{
    [JsonPropertyName("next_token")]
    public string? NextToken { get; set; }

    [JsonPropertyName("total")]
    public int? Total { get; set; }
}