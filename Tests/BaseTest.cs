using NUnit.Framework;
using OpenQA.Selenium;
using Serilog;
using System;
using TakealotAutomation.Configuration;
using TakealotAutomation.Core;

namespace TakealotAutomation.Tests
{
    /// <summary>
    /// Base test class with setup and teardown logic
    /// </summary>
    [TestFixture]
    public abstract class BaseTest
    {
        protected IWebDriver? Driver { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"{ConfigurationManager.GetLogPath()}/log-.txt", rollingInterval: RollingInterval.Day)
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
                TakeScreenshot(TestContext.CurrentContext.Test.Name);
                Log.Error($"Test Failed: {TestContext.CurrentContext.Test.Name}");
            }
            else
            {
                Log.Information($"Test Passed: {TestContext.CurrentContext.Test.Name}");
            }

            DriverFactory.QuitDriver(Driver!);
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
                var screenshotDirectory = ConfigurationManager.GetScreenshotPath();
                if (!Directory.Exists(screenshotDirectory))
                {
                    Directory.CreateDirectory(screenshotDirectory);
                }

                var screenshotPath = Path.Combine(screenshotDirectory, $"{screenshotName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
                var screenshot = ((ITakesScreenshot)Driver!).GetScreenshot();
                screenshot.SaveAsFile(screenshotPath);
                Log.Information($"Screenshot saved: {screenshotPath}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error taking screenshot: {ex.Message}");
            }
        }
    }
}
