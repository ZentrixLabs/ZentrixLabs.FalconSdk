using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ZentrixLabs.FalconSdk.Models;
using ZentrixLabs.FalconSdk.Configuration;
using ZentrixLabs.FalconSdk.Helpers;

namespace ZentrixLabs.FalconSdk.Services
{
    /// <summary>
    /// Provides methods to retrieve server device information from the CrowdStrike Falcon API.
    /// </summary>
    public class CrowdStrikeDeviceService
    {
        private readonly HttpClient _httpClient;
        private readonly CrowdStrikeAuthService _authService;
        private readonly CrowdStrikeOptions _options;
        private readonly ILogger<CrowdStrikeDeviceService> _logger;

        private const string ServerTypeCode = "2";
        private const string DomainControllerTypeCode = "3";

        /// <summary>
        /// Initializes a new instance of the <see cref="CrowdStrikeDeviceService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for API requests.</param>
        /// <param name="authService">The authentication service for obtaining access tokens.</param>
        /// <param name="options">The configuration options for CrowdStrike API.</param>
        /// <param name="logger">The logger instance.</param>
        public CrowdStrikeDeviceService(
            HttpClient httpClient,
            CrowdStrikeAuthService authService,
            IOptions<CrowdStrikeOptions> options,
            ILogger<CrowdStrikeDeviceService> logger)
        {
            _httpClient = httpClient;
            _authService = authService;
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a filtered list of server and domain controller devices from CrowdStrike Falcon.
        /// </summary>
        public async Task<FalconRequestResult<List<DeviceDetail>>> GetAllServerDevicesAsync()
        {
            var result = new FalconRequestResult<List<DeviceDetail>>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = new List<DeviceDetail>()
            };

            try
            {
                var accessToken = await _authService.GetAccessTokenAsync();

                var allDeviceIds = await PaginationHelper.GetOffsetPaginatedAsync(
                    fetchPageAsync: async (offset) =>
                    {
                        var idUrl = $"{_options.BaseUrl}/devices/queries/devices/v1?limit=500&offset={offset}&sort=last_seen.desc";
                        var idRequest = new HttpRequestMessage(HttpMethod.Get, idUrl);
                        idRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        return await _httpClient.SendAsync(idRequest);
                    },
                    parseResponseAsync: async (response) =>
                    {
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadFromJsonAsync<PaginatedResponse<string>>();
                    });

                const int chunkSize = 100;
                var allDevices = new List<DeviceDetail>();

                foreach (var chunk in allDeviceIds.Chunk(chunkSize))
                {
                    var idParams = string.Join("&ids=", chunk);
                    var detailUrl = $"{_options.BaseUrl}/devices/entities/devices/v2?ids={idParams}";
                    var detailRequest = new HttpRequestMessage(HttpMethod.Get, detailUrl);
                    detailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage detailResponse = null!;
                    for (int attempt = 1; attempt <= 3; attempt++)
                    {
                        try
                        {
                            detailResponse = await _httpClient.SendAsync(detailRequest);
                            detailResponse.EnsureSuccessStatusCode();
                            break;
                        }
                        catch (Exception ex) when (attempt < 3)
                        {
                            _logger.LogWarning(ex, "[CrowdStrikeDeviceService] Detail request attempt {Attempt} failed. Retrying...", attempt);
                            await Task.Delay(500);
                        }
                    }

                    var rawJson = await detailResponse.Content.ReadAsStringAsync();
                    _logger.LogDebug("🔎 Raw Device Detail Response: {RawJson}", rawJson);

                    if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, rawJson, "Device Detail Response"))
                    {
                        result.ErrorMessage = "API-level error found in one or more chunks.";
                    }

                    var detailData = JsonSerializer.Deserialize<DeviceDetailEnvelope>(
                        rawJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var validTypes = new[] { "Server", "Domain Controller" };
                    var filtered = detailData?.Resources
                        ?.Where(d => d.ProductType == ServerTypeCode || d.ProductType == DomainControllerTypeCode ||
                                     validTypes.Contains(d.ProductTypeDesc?.Trim(), StringComparer.OrdinalIgnoreCase))
                        .ToList() ?? new List<DeviceDetail>();

                    allDevices.AddRange(filtered);
                }

                result.Data = allDevices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Exception caught in GetAllServerDevicesAsync");
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// Retrieves device IDs (AIDs) that match a given hostname.
        /// </summary>
        public async Task<FalconRequestResult<List<string>>> GetDeviceIdsAsync(string hostname)
        {
            var accessToken = await _authService.GetAccessTokenAsync();

            try
            {
                var ids = await PaginationHelper.GetOffsetPaginatedAsync(
                    fetchPageAsync: async (offset) =>
                    {
                        var url = $"{_options.BaseUrl}/devices/queries/devices/v1?limit=100&offset={offset}&filter=hostname:'{hostname}'";
                        var request = new HttpRequestMessage(HttpMethod.Get, url);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        return await _httpClient.SendAsync(request);
                    },
                    parseResponseAsync: async (response) =>
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        _logger.LogDebug("🔎 Raw Response: {ResponseBody}", responseBody);

                        ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Device ID Query Response");

                        response.EnsureSuccessStatusCode();
                        return JsonSerializer.Deserialize<PaginatedResponse<string>>(responseBody);
                    },
                    pageSize: 100);

                return new FalconRequestResult<List<string>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = ids.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Exception caught in GetDeviceIdsAsync");

                return new FalconRequestResult<List<string>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Exception = ex
                };
            }
        }

        /// <summary>
        /// Retrieves detailed device information for a given device ID (AID).
        /// </summary>
        public async Task<FalconRequestResult<DeviceDetail?>> GetDeviceDetailsAsync(string aid)
        {
            var result = new FalconRequestResult<DeviceDetail?>();

            try
            {
                _logger.LogDebug("🟢 Entering GetDeviceDetailsAsync");
                _logger.LogDebug("🟢 AID length: {Length}", aid?.Length ?? -1);
                _logger.LogDebug("🟢 AID value: {Aid}", aid);

                var accessToken = await _authService.GetAccessTokenAsync();
                _logger.LogDebug("🟢 Access token acquired, length: {Length}", accessToken?.Length ?? -1);

                var url = $"{_options.BaseUrl}/devices/entities/devices/v2?ids={Uri.EscapeDataString(aid ?? string.Empty)}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.SendAsync(request);
                result.StatusCode = response.StatusCode;

                var responseBody = await response.Content.ReadAsStringAsync();
                result.RawResponse = responseBody;

                _logger.LogDebug("🟢 Response body: {ResponseBody}", responseBody);

                if (ApiErrorHelper.LogAndCheckForApiErrors(_logger, responseBody, "Device Detail Response"))
                {
                    result.ErrorMessage = "API-level error found in response body.";
                }

                response.EnsureSuccessStatusCode();

                var detailData = JsonSerializer.Deserialize<DeviceDetailEnvelope>(
                    responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                result.Data = detailData?.Resources?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Exception caught in GetDeviceDetailsAsync");
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Exception = ex;
            }

            return result;
        }
    }
}
