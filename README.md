# Takealot-Automation-Framework
Selenium + NUnit UI automation framework built around the Sauce Demo (Swag Labs) site. The project uses a page object model, centralized configuration, and categorized tests (Smoke, Functional, Unit).

**Tech Stack**
- .NET SDK (solution target)
- Selenium WebDriver
- NUnit
- Serilog
- Moq (unit tests for page behavior)

**Project Structure**
- `src/TakealotAutomation/Configuration` config loading and settings
- `src/TakealotAutomation/Core` base page and shared WebDriver helpers
- `src/TakealotAutomation/Pages` page objects
- `src/TakealotAutomation/Tests` test suites
- `src/TakealotAutomation/Utilities` shared utilities
- `src/TakealotAutomation/appsettings.json` runtime config

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

**Configuration**
Edit `src/TakealotAutomation/appsettings.json`:
- `BaseUrl` defaults to `https://www.saucedemo.com`
- `BrowserType`, `HeadlessMode`, and wait settings control execution
- `ScreenshotPath` and `LogPath` define output locations
