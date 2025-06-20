# ZentrixLabs.FalconSdk

[![NuGet](https://img.shields.io/nuget/v/ZentrixLabs.FalconSdk.svg)](https://www.nuget.org/packages/ZentrixLabs.FalconSdk/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ZentrixLabs.FalconSdk.svg)](https://www.nuget.org/packages/ZentrixLabs.FalconSdk/)

---

A lightweight, MIT-licensed .NET 9 SDK for querying CrowdStrike Falcon data using their OAuth2 API.

> This SDK is designed to simplify local telemetry analysis, patch readiness, and device visibility by abstracting Falcon's token and device API interactions.

---

## ✨ Features

---

✅ Supports:
- OAuth2 token generation from API keys  

- 🔌 Supported API Endpoints

| **Purpose**                      | **Falcon API Endpoint**                                   |
|-----------------------------------|-----------------------------------------------------------|
| Device search                     | `/devices/queries/devices/v1`                             |
| Device details                    | `/devices/entities/devices/v2`                            |
| Host groups                       | `/devices/entities/host-groups/v1`                        |
| Vulnerabilities by filter         | `/spotlight/queries/vulnerabilities/v1`                   |
| Vulnerabilities by ID             | `/spotlight/entities/vulnerabilities/v1`                  |
| Vulnerability hosts               | `/spotlight/combined/hosts/v1`                            |
| Vulnerability remediations        | `/spotlight/combined/remediations/v1`                     |
| Vulnerability counts              | `/spotlight/queries/vulnerabilities/v1`                   |
| Vulnerability host counts         | `/spotlight/queries/hosts/v1`                             |
| Vulnerability remediation counts  | `/spotlight/queries/remediations/v1`                      |
| Vulnerability evaluation logic    | `/spotlight/entities/evaluation-logic/v1`                 |
| Alerts search (IDs)               | `/alerts/queries/alerts/v1`                               |
| Alert details                     | `/alerts/entities/alerts/v2`                              |

---

🚧 Not yet implemented:
- Streaming detections or real-time event subscriptions  
- Threat Graph, incidents, or host group mutations  
- Retry logic or circuit breaker support  
- Built-in structured logging or telemetry

---

## 🛠 Requirements

---

You need:
- A CrowdStrike Falcon API key with the following permissions:
  - **Hosts: Read**
  - **Host Groups: Read**
  - **Assets: Read**
  - **Vulnerabilities: Read**
  - **Alerts: Read**

You can create an API key with these permissions in the Falcon console

## 🛠️ Notes
- Pagination: Some endpoints (e.g., devices/queries, spotlight, alerts) require handling of scroll tokens or next tokens for pagination.
- The user creating the key must have the necessary permissions to grant these scopes (Vulnerability Manager, Device Control, etc.)

## 🔑 Setting Up Your API Key
From the Falcon console:
- Go to **Support > API Clients and Keys**
- Create a new key and grant the above permissions

---

## 🔐 Example: Basic Usage

---

```csharp
var options = new CrowdStrikeOptions
{
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret"
};

var auth = new CrowdStrikeAuthService(options);
var token = await auth.GetTokenAsync();

var deviceService = new CrowdStrikeDeviceService(auth);
var deviceIds = await deviceService.GetDeviceIdsAsync();
var devices = await deviceService.GetDeviceDetailsAsync(deviceIds);

// Spotlight Example
var spotlightService = new CrowdStrikeSpotlightService(httpClient, auth, options, logger);
var vulnIds = await spotlightService.GetVulnerabilityIdsForHostAsync("host-aid");
var vulnDetails = await spotlightService.GetVulnerabilityDetailsAsync("host-aid", vulnIds.Data);

// Alerts Example
var alertService = new AlertService(httpClient, auth, options, logger);
var alertIds = await alertService.GetAlertIdsAsync();
var alertDetails = await alertService.GetAlertDetailsAsync(alertIds.Data);
```

---

## 📦 Install from NuGet

---

```bash
dotnet add package ZentrixLabs.FalconSdk
```

[View on NuGet.org](https://www.nuget.org/packages/ZentrixLabs.FalconSdk/)

---

## 🧪 Test Coverage

---

This SDK is currently distributed without bundled unit tests.  
Community contributions are encouraged — feel free to fork and add coverage using xUnit.

---

## 📝 License

---

This project is licensed under the [MIT License](LICENSE).  
You are free to use, modify, and distribute it — including in commercial products — with attribution.

---

## 🌐 More from ZentrixLabs

Explore our tools, apps, and developer blog at [zentrixlabs.net](https://zentrixlabs.net)

---

Licensed under the [MIT License](LICENSE) by ZentrixLabs.

## 🙏 Acknowledgments

- This SDK would not have been possible without the work already done by the team behind the [PSFalcon](https://github.com/CrowdStrike/psfalcon) module and the Falcon SDK.

We extend our thanks to the CrowdStrike API community for their support and documentation.
---

## Contributing
Pull requests are welcome!  
Please fork the repository, make your changes, and submit a pull request.  
Ensure changes are well-tested and match the project's security-first standards.

This Sdk will continue to evolve to encompass more features and services from the CrowdStrike Falcon API.


If you'd like to support this project:

[![Buy Me A Coffee](Docs/coffee.png)](https://www.buymeacoffee.com/Mainframe79)