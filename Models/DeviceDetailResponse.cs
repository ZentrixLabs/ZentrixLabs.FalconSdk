using System.Text.Json.Serialization;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Represents detailed device information returned by the CrowdStrike Falcon API.
/// </summary>
public class DeviceDetail
{
    /// <summary>
    /// The unique device identifier assigned by Falcon.
    /// </summary>
    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// The CrowdStrike customer identifier (CID) for the device.
    /// </summary>
    [JsonPropertyName("cid")]
    public string Cid { get; set; } = string.Empty;

    /// <summary>
    /// The hostname of the device.
    /// </summary>
    [JsonPropertyName("hostname")]
    public string Hostname { get; set; } = string.Empty;

    /// <summary>
    /// The operating system product name installed on the device.
    /// </summary>
    [JsonPropertyName("os_product_name")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// The version of the operating system running on the device.
    /// </summary>
    [JsonPropertyName("os_version")]
    public string OSVersion { get; set; } = string.Empty;

    /// <summary>
    /// The build number of the operating system.
    /// </summary>
    [JsonPropertyName("os_build")]
    public string OSBuild { get; set; } = string.Empty;

    /// <summary>
    /// The kernel version of the operating system.
    /// </summary>
    [JsonPropertyName("kernel_version")]
    public string KernelVersion { get; set; } = string.Empty;

    /// <summary>
    /// The name of the device platform (e.g., Windows, Mac, Linux).
    /// </summary>
    [JsonPropertyName("platform_name")]
    public string PlatformName { get; set; } = string.Empty;

    /// <summary>
    /// The version of the device platform.
    /// </summary>
    [JsonPropertyName("platform_version")]
    public string? PlatformVersion { get; set; }

    /// <summary>
    /// The version of the Falcon agent installed on the device.
    /// </summary>
    [JsonPropertyName("agent_version")]
    public string? AgentVersion { get; set; }

    /// <summary>
    /// The local time reported by the Falcon agent on the device.
    /// </summary>
    [JsonPropertyName("agent_local_time")]
    public string? AgentLocalTime { get; set; }

    /// <summary>
    /// The product type code of the operating system.
    /// </summary>
    [JsonPropertyName("product_type")]
    public string? ProductType { get; set; }

    /// <summary>
    /// The description of the product type.
    /// </summary>
    [JsonPropertyName("product_type_desc")]
    public string ProductTypeDesc { get; set; } = string.Empty;

    /// <summary>
    /// The list of Falcon groups to which the device belongs.
    /// </summary>
    [JsonPropertyName("group_name")]
    public List<string>? GroupName { get; set; }

    /// <summary>
    /// The timestamp when the device was first seen by Falcon.
    /// </summary>
    [JsonPropertyName("first_seen")]
    public string? FirstSeen { get; set; }

    /// <summary>
    /// The timestamp when the device was last seen by Falcon.
    /// </summary>
    [JsonPropertyName("last_seen")]
    public string? LastSeen { get; set; }

    /// <summary>
    /// The username of the last user to log in to the device.
    /// </summary>
    [JsonPropertyName("last_login_user")]
    public string? LastLoginUser { get; set; }

    /// <summary>
    /// The security identifier (SID) of the last login user.
    /// </summary>
    [JsonPropertyName("last_login_user_sid")]
    public string? LastLoginUserSid { get; set; }

    /// <summary>
    /// The timestamp of the last login event on the device.
    /// </summary>
    [JsonPropertyName("last_login_timestamp")]
    public string? LastLoginTimestamp { get; set; }

    /// <summary>
    /// The timestamp of the last reboot of the device.
    /// </summary>
    [JsonPropertyName("last_reboot")]
    public string? LastReboot { get; set; }

    /// <summary>
    /// The serial number of the device.
    /// </summary>
    [JsonPropertyName("serial_number")]
    public string? SerialNumber { get; set; }

    /// <summary>
    /// The current status of the device in Falcon.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// The provisioning status of the device.
    /// </summary>
    [JsonPropertyName("provision_status")]
    public string? ProvisionStatus { get; set; }

    /// <summary>
    /// The BIOS version of the device.
    /// </summary>
    [JsonPropertyName("bios_version")]
    public string? BiosVersion { get; set; }

    /// <summary>
    /// The manufacturer of the device's BIOS.
    /// </summary>
    [JsonPropertyName("bios_manufacturer")]
    public string? BiosManufacturer { get; set; }

    /// <summary>
    /// The product name of the system hardware.
    /// </summary>
    [JsonPropertyName("system_product_name")]
    public string? SystemProductName { get; set; }

    /// <summary>
    /// The manufacturer of the system hardware.
    /// </summary>
    [JsonPropertyName("system_manufacturer")]
    public string? SystemManufacturer { get; set; }

    /// <summary>
    /// The description of the chassis type (e.g., laptop, desktop).
    /// </summary>
    [JsonPropertyName("chassis_type_desc")]
    public string? ChassisTypeDesc { get; set; }

    /// <summary>
    /// The current IP address used to connect to Falcon.
    /// </summary>
    [JsonPropertyName("connection_ip")]
    public string? ConnectionIp { get; set; }

    /// <summary>
    /// The MAC address used for the current Falcon connection.
    /// </summary>
    [JsonPropertyName("connection_mac_address")]
    public string? ConnectionMacAddress { get; set; }

    /// <summary>
    /// The local IP address of the device.
    /// </summary>
    [JsonPropertyName("local_ip")]
    public string? LocalIp { get; set; }

    /// <summary>
    /// The external (public) IP address of the device.
    /// </summary>
    [JsonPropertyName("external_ip")]
    public string? ExternalIp { get; set; }

    /// <summary>
    /// The IP address of the device's default gateway.
    /// </summary>
    [JsonPropertyName("default_gateway_ip")]
    public string? DefaultGatewayIp { get; set; }

    /// <summary>
    /// The domain to which the device belongs.
    /// </summary>
    [JsonPropertyName("machine_domain")]
    public string? MachineDomain { get; set; }

    /// <summary>
    /// The site or location name associated with the device.
    /// </summary>
    [JsonPropertyName("site_name")]
    public string? SiteName { get; set; }

    /// <summary>
    /// The list of organizational units (OUs) to which the device belongs.
    /// </summary>
    [JsonPropertyName("ou")]
    public List<string>? OrganizationalUnits { get; set; }

    /// <summary>
    /// The list of tags assigned to the device.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Indicates whether the device is operating in reduced functionality mode.
    /// </summary>
    [JsonPropertyName("reduced_functionality_mode")]
    public string? ReducedFunctionalityMode { get; set; }

    /// <summary>
    /// Indicates whether the host is hidden in Falcon.
    /// </summary>
    [JsonPropertyName("host_hidden_status")]
    public string? HostHiddenStatus { get; set; }

    /// <summary>
    /// The status of filesystem containment on the device.
    /// </summary>
    [JsonPropertyName("filesystem_containment_status")]
    public string? FilesystemContainmentStatus { get; set; }

    /// <summary>
    /// The Real Time Response (RTR) state of the device.
    /// </summary>
    [JsonPropertyName("rtr_state")]
    public string? RtrState { get; set; }

    /// <summary>
    /// The pointer size (architecture) of the operating system.
    /// </summary>
    [JsonPropertyName("pointer_size")]
    public string? PointerSize { get; set; }

    /// <summary>
    /// The major version number of the operating system.
    /// </summary>
    [JsonPropertyName("major_version")]
    public string? MajorVersion { get; set; }

    /// <summary>
    /// The minor version number of the operating system.
    /// </summary>
    [JsonPropertyName("minor_version")]
    public string? MinorVersion { get; set; }

    /// <summary>
    /// The build number of the operating system.
    /// </summary>
    [JsonPropertyName("build_number")]
    public string? BuildNumber { get; set; }

    /// <summary>
    /// The dictionary of device policy assignments and their details.
    /// </summary>
    [JsonPropertyName("device_policies")]
    public Dictionary<string, object>? DevicePolicies { get; set; }

    /// <summary>
    /// The list of policy objects applied to the device.
    /// </summary>
    [JsonPropertyName("policies")]
    public List<object>? Policies { get; set; }

    /// <summary>
    /// Additional metadata associated with the device.
    /// </summary>
    [JsonPropertyName("meta")]
    public Dictionary<string, object>? Meta { get; set; }

    /// <summary>
    /// The timestamp when the device details were last modified.
    /// </summary>
    [JsonPropertyName("modified_timestamp")]
    public string? ModifiedTimestamp { get; set; }

    /// <summary>
    /// The base configuration ID for the device.
    /// </summary>
    [JsonPropertyName("config_id_base")]
    public string? ConfigIdBase { get; set; }

    /// <summary>
    /// The build configuration ID for the device.
    /// </summary>
    [JsonPropertyName("config_id_build")]
    public string? ConfigIdBuild { get; set; }

    /// <summary>
    /// The platform configuration ID for the device.
    /// </summary>
    [JsonPropertyName("config_id_platform")]
    public string? ConfigIdPlatform { get; set; }

    /// <summary>
    /// The CPU signature of the device.
    /// </summary>
    [JsonPropertyName("cpu_signature")]
    public string? CpuSignature { get; set; }

    /// <summary>
    /// The CPU vendor of the device.
    /// </summary>
    [JsonPropertyName("cpu_vendor")]
    public string? CpuVendor { get; set; }

    /// <summary>
    /// The MAC address of the device.
    /// </summary>
    [JsonPropertyName("mac_address")]
    public string? MacAddress { get; set; }

    /// <summary>
    /// The hash value representing the group membership of the device.
    /// </summary>
    [JsonPropertyName("group_hash")]
    public string? GroupHash { get; set; }
}

/// <summary>
/// Represents the top-level response envelope containing device detail results and any associated errors.
/// </summary>
public class DeviceDetailEnvelope
{
    /// <summary>
    /// A list of detailed device records returned by the API.
    /// </summary>
    [JsonPropertyName("resources")]
    public List<DeviceDetail> Resources { get; set; } = [];

    /// <summary>
    /// A list of errors encountered during the API request, if any.
    /// </summary>
    [JsonPropertyName("errors")]
    public List<object>? Errors { get; set; }
}
