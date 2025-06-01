using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ZentrixLabs.FalconSdk.Models;
using ZentrixLabs.FalconSdk.Configuration;

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
        public async Task<List<DeviceDetail>> GetAllServerDevicesAsync()
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

            var allDevices = new List<DeviceDetail>();
            const int chunkSize = 100;

            foreach (var chunk in allDeviceIds.Chunk(chunkSize))
            {
                var idParams = string.Join("&ids=", chunk);
                var detailUrl = $"{_options.BaseUrl}/devices/entities/devices/v2?ids={idParams}";
                var detailRequest = new HttpRequestMessage(HttpMethod.Get, detailUrl);
                detailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var detailResponse = await _httpClient.SendAsync(detailRequest);
                detailResponse.EnsureSuccessStatusCode();

                var rawJson = await detailResponse.Content.ReadAsStringAsync();
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

            return allDevices;
        }

        /// <summary>
        /// Retrieves device IDs (AIDs) that match a given hostname.
        /// </summary>
        public async Task<List<string>> GetDeviceIdsAsync(string hostname)
        {
            var accessToken = await _authService.GetAccessTokenAsync();

            return await PaginationHelper.GetOffsetPaginatedAsync(
                fetchPageAsync: async (offset) =>
                {
                    var url = $"{_options.BaseUrl}/devices/queries/devices/v1?limit=100&offset={offset}&filter=hostname:'{hostname}'";
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    return await _httpClient.SendAsync(request);
                },
                parseResponseAsync: async (response) =>
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<PaginatedResponse<string>>();
                },
                pageSize: 100);
        }

        /// <summary>
        /// Retrieves detailed device information for a given device ID (AID).
        /// </summary>
        public async Task<DeviceDetail?> GetDeviceDetailsAsync(string aid)
        {
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
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("🟢 Response body: {ResponseBody}", responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ API returned {StatusCode}", response.StatusCode);
                    response.EnsureSuccessStatusCode();  // throw
                }

                var detailData = JsonSerializer.Deserialize<DeviceDetailEnvelope>(
                    responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return detailData?.Resources?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Exception caught in GetDeviceDetailsAsync");
                throw;
            }
        }
    }
}
