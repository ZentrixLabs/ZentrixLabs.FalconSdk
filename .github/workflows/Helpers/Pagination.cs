using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Helpers;

public static class PaginationHelper
{
    public static async Task<List<T>> GetAllPaginatedAsync<T>(
        Func<string?, Task<HttpResponseMessage>> fetchPageAsync,
        Func<HttpResponseMessage, Task<PaginatedResponse<T>>> parseResponseAsync)
    {
        var allResults = new List<T>();
        string? nextToken = null;

        do
        {
            var response = await fetchPageAsync(nextToken);
            var parsed = await parseResponseAsync(response);

            if (parsed?.Resources != null)
                allResults.AddRange(parsed.Resources);

            nextToken = parsed?.Pagination?.NextToken;
        }
        while (!string.IsNullOrEmpty(nextToken));

        return allResults;
    }
}

public class PaginatedResponse<T>
{
    [JsonPropertyName("resources")]
    public List<T>? Resources { get; set; }

    [JsonPropertyName("meta")]
    public Meta? Meta { get; set; }

    public Pagination? Pagination => Meta?.Pagination;
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

    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    [JsonPropertyName("total")]
    public int? Total { get; set; }
}