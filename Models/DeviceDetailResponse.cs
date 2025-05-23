using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

public class DeviceDetail
{
    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("cid")]
    public string Cid { get; set; } = string.Empty;

    [JsonPropertyName("hostname")]
    public string Hostname { get; set; } = string.Empty;

    [JsonPropertyName("os_product_name")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("os_version")]
    public string OSVersion { get; set; } = string.Empty;

    [JsonPropertyName("os_build")]
    public string OSBuild { get; set; } = string.Empty;

    [JsonPropertyName("kernel_version")]
    public string KernelVersion { get; set; } = string.Empty;

    [JsonPropertyName("platform_name")]
    public string PlatformName { get; set; } = string.Empty;

    [JsonPropertyName("platform_version")]
    public string? PlatformVersion { get; set; }

    [JsonPropertyName("agent_version")]
    public string? AgentVersion { get; set; }

    [JsonPropertyName("agent_local_time")]
    public string? AgentLocalTime { get; set; }

    [JsonPropertyName("product_type")]
    public string? ProductType { get; set; }

    [JsonPropertyName("product_type_desc")]
    public string ProductTypeDesc { get; set; } = string.Empty;

    [JsonPropertyName("group_name")]
    public List<string>? GroupName { get; set; }

    [JsonPropertyName("first_seen")]
    public string? FirstSeen { get; set; }

    [JsonPropertyName("last_seen")]
    public string? LastSeen { get; set; }

    [JsonPropertyName("last_login_user")]
    public string? LastLoginUser { get; set; }

    [JsonPropertyName("last_login_user_sid")]
    public string? LastLoginUserSid { get; set; }

    [JsonPropertyName("last_login_timestamp")]
    public string? LastLoginTimestamp { get; set; }

    [JsonPropertyName("last_reboot")]
    public string? LastReboot { get; set; }

    [JsonPropertyName("serial_number")]
    public string? SerialNumber { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("provision_status")]
    public string? ProvisionStatus { get; set; }

    [JsonPropertyName("bios_version")]
    public string? BiosVersion { get; set; }

    [JsonPropertyName("bios_manufacturer")]
    public string? BiosManufacturer { get; set; }

    [JsonPropertyName("system_product_name")]
    public string? SystemProductName { get; set; }

    [JsonPropertyName("system_manufacturer")]
    public string? SystemManufacturer { get; set; }

    [JsonPropertyName("chassis_type_desc")]
    public string? ChassisTypeDesc { get; set; }

    [JsonPropertyName("connection_ip")]
    public string? ConnectionIp { get; set; }

    [JsonPropertyName("connection_mac_address")]
    public string? ConnectionMacAddress { get; set; }

    [JsonPropertyName("local_ip")]
    public string? LocalIp { get; set; }

    [JsonPropertyName("external_ip")]
    public string? ExternalIp { get; set; }

    [JsonPropertyName("default_gateway_ip")]
    public string? DefaultGatewayIp { get; set; }

    [JsonPropertyName("machine_domain")]
    public string? MachineDomain { get; set; }

    [JsonPropertyName("site_name")]
    public string? SiteName { get; set; }

    [JsonPropertyName("ou")]
    public List<string>? OrganizationalUnits { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("reduced_functionality_mode")]
    public string? ReducedFunctionalityMode { get; set; }

    [JsonPropertyName("host_hidden_status")]
    public string? HostHiddenStatus { get; set; }

    [JsonPropertyName("filesystem_containment_status")]
    public string? FilesystemContainmentStatus { get; set; }

    [JsonPropertyName("rtr_state")]
    public string? RtrState { get; set; }

    [JsonPropertyName("pointer_size")]
    public string? PointerSize { get; set; }

    [JsonPropertyName("major_version")]
    public string? MajorVersion { get; set; }

    [JsonPropertyName("minor_version")]
    public string? MinorVersion { get; set; }

    [JsonPropertyName("build_number")]
    public string? BuildNumber { get; set; }

    [JsonPropertyName("device_policies")]
    public Dictionary<string, object>? DevicePolicies { get; set; }

    [JsonPropertyName("policies")]
    public List<object>? Policies { get; set; }

    [JsonPropertyName("meta")]
    public Dictionary<string, object>? Meta { get; set; }

    [JsonPropertyName("modified_timestamp")]
    public string? ModifiedTimestamp { get; set; }

    [JsonPropertyName("config_id_base")]
    public string? ConfigIdBase { get; set; }

    [JsonPropertyName("config_id_build")]
    public string? ConfigIdBuild { get; set; }

    [JsonPropertyName("config_id_platform")]
    public string? ConfigIdPlatform { get; set; }

    [JsonPropertyName("cpu_signature")]
    public string? CpuSignature { get; set; }

    [JsonPropertyName("cpu_vendor")]
    public string? CpuVendor { get; set; }

    [JsonPropertyName("mac_address")]
    public string? MacAddress { get; set; }

    [JsonPropertyName("group_hash")]
    public string? GroupHash { get; set; }
}

public class DeviceDetailEnvelope
{
    [JsonPropertyName("resources")]
    public List<DeviceDetail> Resources { get; set; } = [];

    [JsonPropertyName("errors")]
    public List<object>? Errors { get; set; }
}


