using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace SauceDemoAutomation.Configuration
{
    /// <summary>
    /// Configuration manager for application settings
    /// </summary>
    public class ConfigurationManager
    {
        private static IConfiguration? _configuration;

        public static IConfiguration GetConfiguration()
        {
            if (_configuration == null)
            {
                var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                                  ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                  ?? "Development";

                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

                _configuration = configBuilder.Build();
            }

            return _configuration;
        }

        private static string? GetEnv(string key) => Environment.GetEnvironmentVariable(key);

        public static string GetBaseUrl()
            => GetEnv("APPSETTINGS__BASEURL") ?? GetConfiguration()["AppSettings:BaseUrl"] ?? "https://www.saucedemo.com";

        public static string GetBrowserType()
            => GetEnv("APPSETTINGS__BROWSERTYPE") ?? GetConfiguration()["AppSettings:BrowserType"] ?? "Chrome";

        public static int GetImplicitWait()
            => int.Parse(GetEnv("APPSETTINGS__IMPLICITWAIT") ?? GetConfiguration()["AppSettings:ImplicitWait"] ?? "10");

        public static int GetExplicitWait()
            => int.Parse(GetEnv("APPSETTINGS__EXPLICITWAIT") ?? GetConfiguration()["AppSettings:ExplicitWait"] ?? "15");

        public static int GetPageLoadTimeout()
            => int.Parse(GetEnv("APPSETTINGS__PAGELOADTIMEOUT") ?? GetConfiguration()["AppSettings:PageLoadTimeout"] ?? "30");

        public static bool IsHeadlessMode()
            => bool.Parse(GetEnv("APPSETTINGS__HEADLESSMODE") ?? GetConfiguration()["AppSettings:HeadlessMode"] ?? "false");

        public static string GetScreenshotPath()
            => GetEnv("APPSETTINGS__SCREENSHOTPATH") ?? GetConfiguration()["AppSettings:ScreenshotPath"] ?? "Screenshots";

        public static string GetLogPath()
            => GetEnv("APPSETTINGS__LOGPATH") ?? GetConfiguration()["AppSettings:LogPath"] ?? "Logs";
    }
}
