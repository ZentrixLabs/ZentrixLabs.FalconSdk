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
- Device query API (`/devices/queries/devices/v1`)  
- Device detail API (`/devices/entities/devices/v2`)  

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


## Contributing
Pull requests are welcome!  
Please fork the repository, make your changes, and submit a pull request.  
Ensure changes are well-tested and match the project's security-first standards.

If you'd like to support this project:

[![Buy Me A Coffee](https://cdn.buymeacoffee.com/buttons/default-orange.png)](https://www.buymeacoffee.com/Mainframe79)
