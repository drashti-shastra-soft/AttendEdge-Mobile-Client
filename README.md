# AttendEdge Mobile Client

A cross-platform **.NET MAUI** attendance app for employees and managers. Users can check in and out with **GPS and selfie verification**, apply for leave, view holidays and shift schedules, and track their attendance history. The app syncs with the **AttendEdge Web API**.

| | |
|---|---|
| **Application ID** | `com.shastrasoft.attendedge` |
| **Version** | 1.0 |
| **Framework** | .NET 9 / .NET MAUI |
| **Current target** | Android (minimum API 26) |

---

## Table of Contents

1. [Features](#features)
2. [Tech Stack](#tech-stack)
3. [Project Structure](#project-structure)
4. [Getting Started](#getting-started)
5. [Configuration](#configuration)
6. [Build and Run](#build-and-run)
7. [Signing](#signing)
8. [App Screens](#app-screens)
9. [Roadmap](#roadmap)
10. [Contributing](#contributing)
11. [License](#license)

---

## Features

- **Secure authentication**: login, forgot password, code verification, and change password
- **Biometric login support** via fingerprint authentication
- **Check-in / check-out** with GPS location and selfie verification
- **Attendance history**: browse and filter past attendance records
- **Leave management**: apply for leave and view leave balance
- **Holiday calendar**
- **Dashboard and charts**: attendance summary with interactive charts
- **My Profile**: personal details and document views (Aadhaar, driving licence)
- **Settings**
- **Notifications and offline support**, synced with the AttendEdge Web API
- **Polished UX**: custom loader, filter popup, tabbed navigation, and custom controls

## Tech Stack

| Area | Technology |
|---|---|
| Framework | .NET 9, .NET MAUI (single project) |
| Language | C# (nullable reference types, implicit usings) |
| Navigation | SimpleToolkit.SimpleShell |
| UI | XAML, Syncfusion MAUI (Charts, DataGrid, Inputs, Toolkit), Mopups |
| Charts | LiveChartsCore (SkiaSharp), Syncfusion Charts |
| Camera / selfie | CommunityToolkit.Maui.Camera, Camera.MAUI |
| Biometrics | Plugin.Fingerprint |
| Dialogs | Acr.UserDialogs.Maui |
| Serialization | Newtonsoft.Json |
| Backend | AttendEdge Web API (REST) |

### NuGet Packages

| Package | Version |
|---|---|
| Acr.UserDialogs.Maui | 9.2.2 |
| Camera.MAUI | 1.5.1 |
| CommunityToolkit.Maui | 9.1.0 |
| CommunityToolkit.Maui.Camera | 3.0.2 |
| LiveChartsCore.SkiaSharpView.Maui | 2.0.0-rc1.2 |
| Microsoft.Extensions.Logging.Debug | 9.0.0 |
| Mopups.Maui | 1.0.0 |
| Newtonsoft.Json | 13.0.3 |
| Plugin.Fingerprint | 2.1.5 |
| SimpleToolkit.Core | 6.0.0 |
| SimpleToolkit.SimpleShell | 5.0.0 |
| SkiaSharp | 2.88.6 |
| Syncfusion.Maui.Charts | 31.2.12 |
| Syncfusion.Maui.DataGrid | 32.1.19 |
| Syncfusion.Maui.Inputs | 32.1.19 |
| Syncfusion.Maui.Toolkit | 1.0.8 |

## Project Structure

```
.
├── APIService/          # API client for the AttendEdge Web API
├── Converter/           # XAML value converters
├── CustomeControls/     # Reusable custom controls (e.g. CustomLoaderView)
├── Handlers/            # Custom MAUI handlers
├── Interface/           # Service and helper interfaces
├── Model/               # Data models
├── Platforms/           # Platform-specific code
├── PopupPages/          # Popups (CustomLoaderPopup, FilterPopup)
├── Properties/
├── Resources/           # App icon, splash, images, fonts, raw assets
├── Services/            # Application services
├── Utility/             # Helpers and shared utilities
├── Views/
│   ├── Account/         #   Login, ForgotPassword, CodeVerification, ChangePassword, TakeSelfie
│   ├── Chart/           #   HomePage
│   ├── MyProfile/       #   Personal, Aadhaar, Driving Licence views
│   └── Tapped/          #   Dashboard, AttendanceList, Leave, LeaveBalance, Holiday, MyProfile, Settings
├── App.xaml / App.xaml.cs
├── MauiProgram.cs
├── AttendEdgeMobileClient.csproj
└── AttendEdgeMobileClient.sln
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- .NET MAUI workload:
  ```bash
  dotnet workload install maui
  ```
- Visual Studio 2022 (latest) with the **.NET Multi-platform App UI development** workload, or VS Code with the **.NET MAUI** extension
- Android SDK with an emulator or a physical device (API 26 or higher)
- A device or emulator with camera and location support for selfie and GPS check-in
- Access to the AttendEdge Web API (base URL and credentials)

### Installation

```bash
# 1. Clone the repository
git clone <repository-url>
cd AttendEdge-Mobile-Client

# 2. Restore dependencies
dotnet restore
```

## Configuration

| Setting | Where | Notes |
|---|---|---|
| API base URL | `APIService/` or `Utility/` | Point to your AttendEdge Web API environment |
| Syncfusion license key | `MauiProgram.cs` | Register at startup if your Syncfusion packages require it |
| Permissions | `Platforms/Android` manifest | Camera, location, and biometric permissions are required |
| Signing credentials | Build parameters or CI secrets | Never commit passwords or keystores |

## Build and Run

```bash
# Debug build
dotnet build -f net9.0-android -c Debug

# Build and run on a connected device or emulator
dotnet build -f net9.0-android -t:Run

# Release build (APK)
dotnet publish -f net9.0-android -c Release
```

In Visual Studio, open `AttendEdgeMobileClient.sln`, select an Android emulator or device, and press **F5**.

## Signing

Release builds must be signed with your own keystore. Keep the keystore and its credentials out of source control and pass them at build time:

```bash
dotnet publish -f net9.0-android -c Release \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=<path-to-keystore> \
  -p:AndroidSigningKeyAlias=<alias> \
  -p:AndroidSigningStorePass=env:KEYSTORE_PASS \
  -p:AndroidSigningKeyPass=env:KEY_PASS
```

iOS builds additionally require a valid Apple distribution certificate and provisioning profile.

## App Screens

| Area | Screens |
|---|---|
| Account | Login, Forgot Password, Code Verification, Change Password, Take Selfie |
| Home | Dashboard, Charts |
| Attendance | Attendance List with filter popup |
| Leave | Apply Leave, Leave Balance, Holidays |
| Profile | Personal details, Aadhaar, Driving Licence, Settings |

## Roadmap

- [ ] Enable and test the iOS target
- [ ] Add unit tests for services
- [ ] Set up CI (build and test) with GitHub Actions
- [ ] Move environment configuration and signing credentials out of source control

## Contributing

1. Fork the project
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m "Add your feature"`
4. Push the branch: `git push origin feature/your-feature`
5. Open a Pull Request

## License

License to be defined. Add a `LICENSE` file to specify usage terms.
