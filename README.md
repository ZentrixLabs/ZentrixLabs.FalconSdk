# ZentrixLabs.FalconSdk

A lightweight, MIT-licensed .NET 9 SDK for querying CrowdStrike Falcon data using their OAuth2 API.

> This SDK is designed to simplify local telemetry analysis, patch readiness, and device visibility by abstracting Falcon's token and device API interactions.

---

## ✨ Features

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

You need:
- A CrowdStrike Falcon API key with the following permissions:
  - **Hosts: Read**
  - **Host Groups: Read**

From the Falcon console:
- Go to **Support > API Clients and Keys**
- Create a new key and grant the above permissions

---

## 🔐 Example: Basic Usage

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

## 📦 Install

This project is not yet published to NuGet. To use it locally:

```bash
dotnet add package ZentrixLabs.FalconSdk --source ./bin/Release
```

Or build manually:

```bash
dotnet build
dotnet pack -c Release
```
The `.nupkg` file will appear in `./bin/Release` for local installation.

---

## 🧪 Test Coverage

No unit tests are currently included.  
Developers are welcome to add tests using xUnit and reference the SDK as needed.

---

## 📝 License

MIT © [ZentrixLabs](https://github.com/ZentrixLabs/ZentrixLabs.FalconSdk)

Contributions welcome. Open a PR or issue if you find bugs or want to extend support.
