# SauceDemo-Automation-Framework
Selenium + NUnit UI automation framework built around the Sauce Demo (Swag Labs) site. The project uses a page object model, centralized configuration, and categorized tests (Smoke, Functional, Unit).

**Tech Stack**
- .NET SDK (solution target)
- Selenium WebDriver
- NUnit
- Serilog
- Moq (unit tests for page behavior)

**Project Structure**
- `src/SauceDemoAutomation/Configuration` config loading and settings
- `src/SauceDemoAutomation/Core` base page and shared WebDriver helpers
- `src/SauceDemoAutomation/Pages` page objects
- `src/SauceDemoAutomation/Tests` test suites
- `src/SauceDemoAutomation/Utilities` shared utilities
- `src/SauceDemoAutomation/appsettings.json` runtime config

**Quick Start**
1. Install the .NET SDK that matches the solution.
2. Restore and build.

```powershell
dotnet restore
dotnet build
```

**Run Tests**
```powershell
dotnet test
```

Run by category (examples):
```powershell
dotnet test --filter TestCategory=Smoke
dotnet test --filter TestCategory=Functional
dotnet test --filter TestCategory=Unit
```

**CI**
- GitHub Actions runs restore/build/test on `windows-latest`.
- Test results are saved to `TestResults` and uploaded as artifacts (TRX, logs, screenshots).

**Configuration**
Edit `src/SauceDemoAutomation/appsettings.json`:
- `BaseUrl` defaults to `https://www.saucedemo.com`
- `BrowserType`, `HeadlessMode`, and wait settings control execution
- `ScreenshotPath` and `LogPath` define output locations

Environment-specific overrides:
- `appsettings.Development.json`
- `appsettings.CI.json`

Set the environment before running:
```powershell
$env:DOTNET_ENVIRONMENT="Development"
dotnet test
```

**Reports**
`dotnet test` produces TRX files. In CI these are collected from `TestResults`.
