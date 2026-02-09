using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace TakealotAutomation.Configuration
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
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                _configuration = configBuilder.Build();
            }

            return _configuration;
        }

        public static string GetBaseUrl() => GetConfiguration()["AppSettings:BaseUrl"] ?? "https://www.takealot.com";

        public static string GetBrowserType() => GetConfiguration()["AppSettings:BrowserType"] ?? "Chrome";

        public static int GetImplicitWait() => int.Parse(GetConfiguration()["AppSettings:ImplicitWait"] ?? "10");

        public static int GetExplicitWait() => int.Parse(GetConfiguration()["AppSettings:ExplicitWait"] ?? "15");

        public static int GetPageLoadTimeout() => int.Parse(GetConfiguration()["AppSettings:PageLoadTimeout"] ?? "30");

        public static bool IsHeadlessMode() => bool.Parse(GetConfiguration()["AppSettings:HeadlessMode"] ?? "false");

        public static string GetScreenshotPath() => GetConfiguration()["AppSettings:ScreenshotPath"] ?? "Screenshots";

        public static string GetLogPath() => GetConfiguration()["AppSettings:LogPath"] ?? "Logs";
    }
}
