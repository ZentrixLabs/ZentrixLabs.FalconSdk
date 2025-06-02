using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Helpers;

public static class PaginationHelper
{
    private static async Task<T> RetryOnFailureAsync<T>(Func<Task<T>> action, int maxRetries = 3, int delayMilliseconds = 500)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                return await action();
            }
            catch when (attempt < maxRetries - 1)
            {
                await Task.Delay(delayMilliseconds);
            }
        }

        // Last attempt, let the exception bubble up
        return await action();
    }

    public static async Task<List<T>> GetTokenPaginatedAsync<T>(
        Func<string?, Task<HttpResponseMessage>> fetchPageAsync,
        Func<HttpResponseMessage, Task<PaginatedResponse<T>>> parseResponseAsync)
    {
        var allResults = new List<T>();
        string? nextToken = null;

        do
        {
            var response = await RetryOnFailureAsync(() => fetchPageAsync(nextToken));
            var parsed = await RetryOnFailureAsync(() => parseResponseAsync(response));

            if (parsed?.Resources != null)
                allResults.AddRange(parsed.Resources);

            nextToken = parsed?.Pagination?.NextToken;
        }
        while (!string.IsNullOrEmpty(nextToken));

        return allResults;
    }

    public static async Task<List<T>> GetOffsetPaginatedAsync<T>(
        Func<int, Task<HttpResponseMessage>> fetchPageAsync,
        Func<HttpResponseMessage, Task<PaginatedResponse<T>>> parseResponseAsync,
        int pageSize = 500)
    {
        var allResults = new List<T>();
        int offset = 0;
        int? total = null;

        do
        {
            var response = await RetryOnFailureAsync(() => fetchPageAsync(offset));
            var parsed = await RetryOnFailureAsync(() => parseResponseAsync(response));

            if (parsed?.Resources != null)
                allResults.AddRange(parsed.Resources);

            total = parsed?.Pagination?.Total ?? total;
            offset += pageSize;
        }
        while (total == null || offset < total);

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