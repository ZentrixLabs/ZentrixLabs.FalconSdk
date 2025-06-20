using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ZentrixLabs.FalconSdk.Configuration;
using ZentrixLabs.FalconSdk.Helpers;
using ZentrixLabs.FalconSdk.Models;

namespace ZentrixLabs.FalconSdk.Services;

/// <summary>
/// Provides methods to interact with CrowdStrike Falcon Alerts API.
/// </summary>
public class AlertService
{
    private readonly HttpClient _httpClient;
    private readonly CrowdStrikeAuthService _authService;
    private readonly CrowdStrikeOptions _options;
    private readonly ILogger<AlertService> _logger;

    public AlertService(
        HttpClient httpClient,
        CrowdStrikeAuthService authService,
        IOptions<CrowdStrikeOptions> options,
        ILogger<AlertService> logger)
    {
        _httpClient = httpClient;
        _authService = authService;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a list of alert IDs matching the specified filter.
    /// </summary>
    public async Task<FalconRequestResult<List<string>>> GetAlertIdsAsync(string? filter = null)
    {
        var result = new FalconRequestResult<List<string>>();
        try
        {
            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{_options.BaseUrl}/alerts/queries/alerts/v1";
            if (!string.IsNullOrEmpty(filter))
                url += $"?filter={Uri.EscapeDataString(filter)}";

            var ids = await PaginationHelper.GetAllPaginatedAsync(
                fetchPageAsync: async (nextToken) =>
                {
                    var pageUrl = string.IsNullOrEmpty(nextToken)
                        ? url
                        : $"{url}&next_token={Uri.EscapeDataString(nextToken)}";
                    return await _httpClient.GetAsync(pageUrl);
                },
                parseResponseAsync: async (response) =>
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    result.RawResponse = responseBody;

                    if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Alerts API"))
                        result.ErrorMessage = "API-level error found in response.";

                    response.EnsureSuccessStatusCode();
                    return JsonSerializer.Deserialize<PaginatedResponse<string>>(responseBody);
                });

            result.Data = ids;
            result.StatusCode = System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Exception in GetAlertIdsAsync");
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Exception = ex;
        }
        return result;
    }

    /// <summary>
    /// Retrieves detailed information about alerts by their IDs.
    /// </summary>
    public async Task<FalconRequestResult<List<AlertDetail>>> GetAlertDetailsAsync(List<string> alertIds)
    {
        var result = new FalconRequestResult<List<AlertDetail>>();
        try
        {
            if (alertIds == null || alertIds.Count == 0)
            {
                result.Data = new List<AlertDetail>();
                result.StatusCode = System.Net.HttpStatusCode.OK;
                return result;
            }

            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var quotedIds = alertIds.ConvertAll(id => $"'{id}'");
            var filter = $"ids:[{string.Join(",", quotedIds)}]";
            var url = $"{_options.BaseUrl}/alerts/entities/alerts/v2?filter={Uri.EscapeDataString(filter)}";

            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();
            result.RawResponse = responseBody;

            if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Alerts API"))
                result.ErrorMessage = "API-level error found in response.";

            response.EnsureSuccessStatusCode();
            var parsed = JsonSerializer.Deserialize<PaginatedResponse<AlertDetail>>(responseBody);
            result.Data = parsed?.Resources ?? new List<AlertDetail>();
            result.StatusCode = System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Exception in GetAlertDetailsAsync");
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Exception = ex;
        }
        return result;
    }
}