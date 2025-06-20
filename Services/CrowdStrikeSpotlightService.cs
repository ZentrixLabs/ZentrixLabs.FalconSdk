using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ZentrixLabs.FalconSdk.Configuration;
using ZentrixLabs.FalconSdk.Models;
using Microsoft.Extensions.Logging;
using ZentrixLabs.FalconSdk.Helpers;

namespace ZentrixLabs.FalconSdk.Services;

/// <summary>
/// Provides methods to interact with CrowdStrike Spotlight API for retrieving vulnerability data.
/// </summary>
public class CrowdStrikeSpotlightService
{
    private readonly HttpClient _httpClient;
    private readonly CrowdStrikeAuthService _authService;
    private readonly CrowdStrikeOptions _options;
    private readonly ILogger<CrowdStrikeSpotlightService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrowdStrikeSpotlightService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used for API requests.</param>
    /// <param name="authService">The authentication service for obtaining access tokens.</param>
    /// <param name="options">The configuration options for CrowdStrike API.</param>
    /// <param name="logger">The logger instance.</param>
    public CrowdStrikeSpotlightService(
        HttpClient httpClient,
        CrowdStrikeAuthService authService,
        IOptions<CrowdStrikeOptions> options,
        ILogger<CrowdStrikeSpotlightService> logger)
    {
        _httpClient = httpClient;
        _authService = authService;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a list of vulnerability IDs for a specific host.
    /// </summary>
    /// <param name="aid">The host ID (AID) to query vulnerabilities for.</param>
    /// <returns>A list of vulnerability IDs.</returns>
    public async Task<FalconRequestResult<List<string>>> GetVulnerabilityIdsForHostAsync(string aid)
    {
        var result = new FalconRequestResult<List<string>>();
        try
        {
            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var ids = await PaginationHelper.GetAllPaginatedAsync(
                fetchPageAsync: async (nextToken) =>
                {
                    var baseUrl = $"{_options.BaseUrl}/spotlight/queries/vulnerabilities/v1?filter=aid:'{aid}'";
                    var url = string.IsNullOrEmpty(nextToken)
                        ? baseUrl
                        : $"{baseUrl}&next_token={Uri.EscapeDataString(nextToken)}";

                    return await _httpClient.GetAsync(url);
                },
                parseResponseAsync: async (response) =>
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    result.RawResponse = responseBody;

                    if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Spotlight API"))
                    {
                        result.ErrorMessage = "API-level error found in response.";
                    }

                    response.EnsureSuccessStatusCode();
                    return JsonSerializer.Deserialize<PaginatedResponse<string>>(responseBody);
                });

            result.Data = ids;
            result.StatusCode = System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Exception in GetVulnerabilityIdsForHostAsync");
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Exception = ex;
        }

        return result;
    }

    /// <summary>
    /// Retrieves detailed information about vulnerabilities based on their IDs.
    /// </summary>
    /// <param name="aid">The host ID (AID) to query vulnerabilities for.</param>
    /// <param name="vulnIds">A list of vulnerability IDs to retrieve details for.</param>
    /// <param name="useHostSpecific">Indicates whether to use host-specific vulnerability IDs prefixed with the host ID (AID).</param>
    /// <param name="filter">An optional filter string for querying vulnerabilities.</param>
    /// <param name="useFacets">Indicates whether to include facets like remediation or evaluation logic in the query.</param>
    /// <returns>A list of <see cref="VulnerabilityDetail"/> objects containing detailed information about the vulnerabilities.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task<FalconRequestResult<List<VulnerabilityDetail>>> GetVulnerabilityDetailsAsync(
        string aid,
        List<string>? vulnIds = null,
        bool useHostSpecific = true,
        string? filter = null,
        bool useFacets = false)
    {
        var result = new FalconRequestResult<List<VulnerabilityDetail>>();

        try
        {
            vulnIds ??= new List<string>();

            if (string.IsNullOrEmpty(filter))
            {
                if (vulnIds.Count > 0)
                {
                    var quotedIds = vulnIds.Select(id => $"'{id}'");
                    filter = $"ids:[{string.Join(",", quotedIds)}]";
                }
                else
                {
                    filter = $"aid:'{aid}'";
                }
            }

            _logger.LogDebug("🔗 Querying vulnerabilities with filter: {Filter}", filter);

            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var details = await PaginationHelper.GetAllPaginatedAsync(
                fetchPageAsync: async (nextToken) =>
                {
                    var baseUrl = $"{_options.BaseUrl}/spotlight/combined/vulnerabilities/v1?filter={Uri.EscapeDataString(filter)}";
                    var url = string.IsNullOrEmpty(nextToken)
                        ? baseUrl
                        : $"{baseUrl}&next_token={Uri.EscapeDataString(nextToken)}";

                    if (useFacets)
                    {
                        url += "&facet=remediation,evaluation_logic";
                    }

                    return await _httpClient.GetAsync(url);
                },
                parseResponseAsync: async (response) =>
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    result.RawResponse = responseBody;

                    if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Spotlight API"))
                    {
                        result.ErrorMessage = "API-level error found in response.";
                    }

                    response.EnsureSuccessStatusCode();
                    return JsonSerializer.Deserialize<PaginatedResponse<VulnerabilityDetail>>(responseBody);
                });

            result.Data = details;
            result.StatusCode = System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Exception in GetVulnerabilityDetailsAsync");
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Exception = ex;
        }

        return result;
    }

    /// <summary>
    /// Retrieves a list of vulnerability hosts matching the specified filter.
    /// </summary>
    public async Task<FalconRequestResult<List<VulnerabilityHost>>> GetVulnerabilityHostsAsync(string? filter = null)
    {
        var result = new FalconRequestResult<List<VulnerabilityHost>>();
        try
        {
            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{_options.BaseUrl}/spotlight/combined/hosts/v1";
            if (!string.IsNullOrEmpty(filter))
                url += $"?filter={Uri.EscapeDataString(filter)}";

            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();
            result.RawResponse = responseBody;

            if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Spotlight API"))
                result.ErrorMessage = "API-level error found in response.";

            response.EnsureSuccessStatusCode();
            var parsed = JsonSerializer.Deserialize<PaginatedResponse<VulnerabilityHost>>(responseBody);
            result.Data = parsed?.Resources ?? new List<VulnerabilityHost>();
            result.StatusCode = System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Exception in GetVulnerabilityHostsAsync");
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Exception = ex;
        }
        return result;
    }

    /// <summary>
    /// Retrieves a list of vulnerability remediations.
    /// </summary>
    public async Task<FalconRequestResult<List<VulnerabilityRemediation>>> GetVulnerabilityRemediationsAsync(string? filter = null)
    {
        var result = new FalconRequestResult<List<VulnerabilityRemediation>>();
        try
        {
            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{_options.BaseUrl}/spotlight/combined/remediations/v1";
            if (!string.IsNullOrEmpty(filter))
                url += $"?filter={Uri.EscapeDataString(filter)}";

            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();
            result.RawResponse = responseBody;

            if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Spotlight API"))
                result.ErrorMessage = "API-level error found in response.";

            response.EnsureSuccessStatusCode();
            var parsed = JsonSerializer.Deserialize<PaginatedResponse<VulnerabilityRemediation>>(responseBody);
            result.Data = parsed?.Resources ?? new List<VulnerabilityRemediation>();
            result.StatusCode = System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Exception in GetVulnerabilityRemediationsAsync");
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Exception = ex;
        }
        return result;
    }

    /// <summary>
    /// Retrieves vulnerability count based on filter.
    /// </summary>
    public async Task<FalconRequestResult<int>> GetVulnerabilityCountAsync(string? filter = null)
    {
        var result = new FalconRequestResult<int>();
        try
        {
            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{_options.BaseUrl}/spotlight/queries/vulnerabilities/v1";
            if (!string.IsNullOrEmpty(filter))
                url += $"?filter={Uri.EscapeDataString(filter)}";

            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();
            result.RawResponse = responseBody;

            if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Spotlight API"))
                result.ErrorMessage = "API-level error found in response.";

            response.EnsureSuccessStatusCode();
            var parsed = JsonSerializer.Deserialize<PaginatedResponse<string>>(responseBody);
            result.Data = parsed?.Meta?.Pagination?.Total ?? 0;
            result.StatusCode = System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Exception in GetVulnerabilityCountAsync");
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Exception = ex;
        }
        return result;
    }

    /// <summary>
    /// Retrieves vulnerability host count based on filter.
    /// </summary>
    public async Task<FalconRequestResult<int>> GetVulnerabilityHostCountAsync(string? filter = null)
    {
        var result = new FalconRequestResult<int>();
        try
        {
            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{_options.BaseUrl}/spotlight/queries/hosts/v1";
            if (!string.IsNullOrEmpty(filter))
                url += $"?filter={Uri.EscapeDataString(filter)}";

            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();
            result.RawResponse = responseBody;

            if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Spotlight API"))
                result.ErrorMessage = "API-level error found in response.";

            response.EnsureSuccessStatusCode();
            var parsed = JsonSerializer.Deserialize<PaginatedResponse<string>>(responseBody);
            result.Data = parsed?.Meta?.Pagination?.Total ?? 0;
            result.StatusCode = System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Exception in GetVulnerabilityHostCountAsync");
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Exception = ex;
        }
        return result;
    }

    /// <summary>
    /// Retrieves vulnerability remediation count based on filter.
    /// </summary>
    public async Task<FalconRequestResult<int>> GetVulnerabilityRemediationCountAsync(string? filter = null)
    {
        var result = new FalconRequestResult<int>();
        try
        {
            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{_options.BaseUrl}/spotlight/queries/remediations/v1";
            if (!string.IsNullOrEmpty(filter))
                url += $"?filter={Uri.EscapeDataString(filter)}";

            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();
            result.RawResponse = responseBody;

            if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Spotlight API"))
                result.ErrorMessage = "API-level error found in response.";

            response.EnsureSuccessStatusCode();
            var parsed = JsonSerializer.Deserialize<PaginatedResponse<string>>(responseBody);
            result.Data = parsed?.Meta?.Pagination?.Total ?? 0;
            result.StatusCode = System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Exception in GetVulnerabilityRemediationCountAsync");
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Exception = ex;
        }
        return result;
    }

    /// <summary>
    /// Retrieves evaluation logic for vulnerabilities.
    /// </summary>
    public async Task<FalconRequestResult<List<EvaluationLogic>>> GetVulnerabilityEvaluationLogicAsync()
    {
        var result = new FalconRequestResult<List<EvaluationLogic>>();
        try
        {
            var accessToken = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{_options.BaseUrl}/spotlight/entities/evaluation-logic/v1";
            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();
            result.RawResponse = responseBody;

            if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Spotlight API"))
                result.ErrorMessage = "API-level error found in response.";

            response.EnsureSuccessStatusCode();
            var parsed = JsonSerializer.Deserialize<PaginatedResponse<EvaluationLogic>>(responseBody);
            result.Data = parsed?.Resources ?? new List<EvaluationLogic>();
            result.StatusCode = System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Exception in GetVulnerabilityEvaluationLogicAsync");
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Exception = ex;
        }
        return result;
    }
}
