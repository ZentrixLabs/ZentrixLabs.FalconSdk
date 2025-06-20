using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZentrixLabs.FalconSdk.Helpers;

public static class PaginationHelper
{
    /// <summary>
    /// Handles paginated API responses using a next_token pattern.
    /// </summary>
    public static async Task<List<T>> GetAllPaginatedAsync<T>(
        Func<string?, Task<HttpResponseMessage>> fetchPageAsync,
        Func<HttpResponseMessage, Task<PaginatedResponse<T>>> parseResponseAsync)
    {
        var results = new List<T>();
        string? nextToken = null;
        do
        {
            var response = await fetchPageAsync(nextToken);
            var page = await parseResponseAsync(response);
            if (page?.Resources != null)
                results.AddRange(page.Resources);

            nextToken = page?.Meta?.Pagination?.NextToken;
        } while (!string.IsNullOrEmpty(nextToken));
        return results;
    }
}