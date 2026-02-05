using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Serilog;
using System;
using System.Net;
using TakealotAutomation.Configuration;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace TakealotAutomation.Core
{
    /// <summary>
    /// Driver factory for managing WebDriver instances
    /// </summary>
    public class DriverFactory
    {
        public static IWebDriver CreateDriver(string browserType = "Chrome")
        {
            IWebDriver? driver = browserType.ToLower() switch
            {
                "chrome" => InitializeChromeDriver(),
                _ => throw new ArgumentException($"Browser type '{browserType}' is not supported")
            };

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(ConfigurationManager.GetImplicitWait());
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(ConfigurationManager.GetPageLoadTimeout());
            driver.Navigate().GoToUrl(ConfigurationManager.GetBaseUrl());

            Log.Information("WebDriver initialized successfully");
            return driver;
        }

        private static IWebDriver InitializeChromeDriver()
        {
            try
            {
                // Try to setup driver using WebDriverManager (for environments with internet)
                new DriverManager().SetUpDriver(new ChromeConfig());
                Log.Information("ChromeDriver configured via WebDriverManager");
            }
            catch (WebException)
            {
                Log.Information("Network unavailable, using bundled ChromeDriver from NuGet package");
            }
            catch (Exception ex)
            {
                Log.Information($"WebDriverManager unavailable ({ex.GetType().Name}), using bundled ChromeDriver");
            }

            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-plugins");
            options.AddExcludedArgument("enable-automation");

            if (ConfigurationManager.IsHeadlessMode())
            {
                options.AddArgument("--headless=new");
            }

            return new ChromeDriver(options);
        }

        public static void QuitDriver(IWebDriver driver)
        {
            if (driver != null)
            {
                try
                {
                    driver.Quit();
                    driver.Dispose();
                    Log.Information("WebDriver quit successfully");
                }
                catch (Exception ex)
                {
                    Log.Error($"Error quitting WebDriver: {ex.Message}");
                }
            }
        }
    }
}
