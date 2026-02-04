using OpenQA.Selenium;
using Serilog;
using System;

namespace TakealotAutomation.Utilities
{
    /// <summary>
    /// Utility class for common WebDriver operations
    /// </summary>
    public class WaitHelper
    {
        /// <summary>
        /// Pauses execution for specified milliseconds
        /// </summary>
        public static void Wait(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
            Log.Information($"Waited for {milliseconds}ms");
        }

        /// <summary>
        /// Waits for element to disappear
        /// </summary>
        public static bool WaitForElementToDisappear(IWebDriver driver, By locator, int timeoutSeconds = 10)
        {
            try
            {
                OpenQA.Selenium.Support.UI.WebDriverWait wait =
                    new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                wait.Until(d =>
                {
                    try
                    {
                        return !d.FindElement(locator).Displayed;
                    }
                    catch (NoSuchElementException)
                    {
                        return true;
                    }
                });
                Log.Information($"Element disappeared: {locator}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error waiting for element to disappear: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retries an action with specified number of attempts
        /// </summary>
        public static bool RetryAction(Action action, int maxAttempts = 3, int delayMs = 1000)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    action.Invoke();
                    Log.Information($"Action succeeded on attempt {i + 1}");
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Warning($"Attempt {i + 1} failed: {ex.Message}");
                    if (i < maxAttempts - 1)
                        Wait(delayMs);
                }
            }
            return false;
        }
    }
}
