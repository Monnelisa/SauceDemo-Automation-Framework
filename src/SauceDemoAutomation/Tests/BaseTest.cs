using NUnit.Framework;
using OpenQA.Selenium;
using Serilog;
using System;
using System.IO;
using SauceDemoAutomation.Configuration;
using SauceDemoAutomation.Core;

namespace SauceDemoAutomation.Tests
{
    /// <summary>
    /// Base test class with setup and teardown logic
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public abstract class BaseTest
    {
        protected IWebDriver? Driver { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var logPath = ResolveOutputPath(ConfigurationManager.GetLogPath());
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"{logPath}/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Test Suite Started");

        }

        [SetUp]
        public void SetUp()
        {
            Log.Information($"Test Started: {TestContext.CurrentContext.Test.Name}");
            Driver = DriverFactory.CreateDriver(ConfigurationManager.GetBrowserType());
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var testName = TestContext.CurrentContext.Test.Name;
                TakeScreenshot(testName);
                SavePageSource(testName);
                Log.Error($"Test Failed: {testName} - {TestContext.CurrentContext.Result.Message}");
            }
            else
            {
                Log.Information($"Test Passed: {TestContext.CurrentContext.Test.Name}");
            }

            DriverFactory.QuitDriver(Driver!);
            Driver = null;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Log.Information("Test Suite Completed");
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Takes a screenshot for debugging purposes
        /// </summary>
        protected void TakeScreenshot(string screenshotName)
        {
            try
            {
                if (Driver == null)
                {
                    Log.Warning("Screenshot skipped because WebDriver is not initialized");
                    return;
                }

                var screenshotDirectory = ResolveOutputPath(ConfigurationManager.GetScreenshotPath());
                if (!Directory.Exists(screenshotDirectory))
                {
                    Directory.CreateDirectory(screenshotDirectory);
                }

                var screenshotPath = Path.Combine(screenshotDirectory, $"{screenshotName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                screenshot.SaveAsFile(screenshotPath);
                Log.Information($"Screenshot saved: {screenshotPath}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error taking screenshot: {ex.Message}");
            }
        }

        protected void SavePageSource(string testName)
        {
            try
            {
                if (Driver == null)
                {
                    Log.Warning("Page source skipped because WebDriver is not initialized");
                    return;
                }

                var outputDirectory = ResolveOutputPath(ConfigurationManager.GetScreenshotPath());
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                var sourcePath = Path.Combine(outputDirectory, $"{testName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.html");
                File.WriteAllText(sourcePath, Driver.PageSource);
                Log.Information($"Page source saved: {sourcePath}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error saving page source: {ex.Message}");
            }
        }

        private static string ResolveOutputPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }

            return Path.Combine(Directory.GetCurrentDirectory(), path);
        }
    }
}
