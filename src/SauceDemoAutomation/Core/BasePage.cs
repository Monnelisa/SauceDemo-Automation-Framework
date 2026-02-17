using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using SauceDemoAutomation.Configuration;     

namespace SauceDemoAutomation.Core
{
    /// <summary>
    /// Base class for all page objects with common wait and interaction methods
    /// </summary>
    public abstract class BasePage
    {
        protected IWebDriver Driver { get; set; }
        protected WebDriverWait Wait { get; set; }

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(ConfigurationManager.GetExplicitWait()));
        }

        /// <summary>
        /// Waits for an element to be visible and returns it
        /// </summary>
        protected IWebElement WaitForElementToBeVisible(By locator)
        {
            try
            {
                var element = Wait.Until(driver =>
                {
                    try
                    {
                        var el = driver.FindElement(locator);
                        return el.Displayed ? el : null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return null;
                    }
                });

                if (element == null)
                {
                    throw new WebDriverTimeoutException($"Element not visible: {locator}");
                }

                Log.Information($"Element found: {locator}");
                return element;
            }
            catch (WebDriverTimeoutException ex)
            {
                Log.Error($"Timeout waiting for element: {locator}");
                throw new TimeoutException($"Element not found within timeout: {locator}", ex);
            }
        }

        /// <summary>
        /// Waits for an element to be clickable and clicks it
        /// </summary>
        protected void WaitAndClick(By locator)
        {
            try
            {
                var element = Wait.Until(driver =>
                {
                    try
                    {
                        var el = driver.FindElement(locator);
                        return (el.Displayed && el.Enabled) ? el : null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return null;
                    }
                });

                if (element == null)
                {
                    throw new WebDriverTimeoutException($"Element not clickable: {locator}");
                }

                try
                {
                    element.Click();
                    Log.Information($"Clicked element: {locator}");
                }
                catch (Exception ex) when (ex is ElementClickInterceptedException || ex is WebDriverException || ex is StaleElementReferenceException)
                {
                    IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)Driver;
                    jsExecutor.ExecuteScript("arguments[0].click();", element);
                    Log.Information($"Clicked element with JS fallback: {locator}");
                }
            }
            catch (WebDriverTimeoutException)
            {
                // Last attempt: click a present element via JS in case visibility checks are too strict in headless CI.
                var elements = Driver.FindElements(locator);
                if (elements.Count > 0)
                {
                    try
                    {
                        IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)Driver;
                        jsExecutor.ExecuteScript("arguments[0].click();", elements[0]);
                        Log.Information($"Clicked present element with JS timeout fallback: {locator}");
                        return;
                    }
                    catch (Exception jsEx)
                    {
                        Log.Error($"JS timeout fallback failed for element: {locator} - {jsEx.Message}");
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"Error clicking element: {locator} - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Scrolls to an element and clicks it with JS fallback on timeout or interception
        /// </summary>
        protected void WaitAndClickWithScroll(By locator)
        {
            try
            {
                ScrollToElement(locator);
            }
            catch (Exception ex)
            {
                Log.Warning($"Scroll failed for element: {locator} - {ex.Message}");
            }

            try
            {
                WaitAndClick(locator);
            }
            catch (Exception ex) when (ex is WebDriverTimeoutException || ex is ElementClickInterceptedException || ex is WebDriverException)
            {
                try
                {
                    var element = WaitForElementToBeVisible(locator);
                    IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)Driver;
                    jsExecutor.ExecuteScript("arguments[0].click();", element);
                    Log.Information($"Clicked element with JS fallback: {locator}");
                }
                catch (Exception jsEx)
                {
                    Log.Error($"Error clicking element with JS fallback: {locator} - {jsEx.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Sends keys to an element after waiting for it to be visible
        /// </summary>
        protected void WaitAndSendKeys(By locator, string text)
        {
            try
            {
                var element = WaitForElementToBeVisible(locator);
                element.Clear();
                element.SendKeys(text);
                Log.Information($"Sent keys to element: {locator}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error sending keys to element: {locator} - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets text from an element
        /// </summary>
        protected string GetElementText(By locator)
        {
            try
            {
                var element = WaitForElementToBeVisible(locator);
                string text = element.Text;
                Log.Information($"Retrieved text from element: {locator} - {text}");
                return text;
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting text from element: {locator} - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Checks if an element is present on the page
        /// </summary>
        protected bool IsElementPresent(By locator)
        {
            return Driver.FindElements(locator).Count > 0;
        }

        /// <summary>
        /// Scrolls to an element
        /// </summary>
        protected void ScrollToElement(By locator)
        {
            try
            {
                var element = Driver.FindElement(locator);
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)Driver;
                jsExecutor.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                Log.Information($"Scrolled to element: {locator}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error scrolling to element: {locator} - {ex.Message}");
            }
        }

        /// <summary>
        /// Waits for page title to be present
        /// </summary>
        protected void WaitForPageTitle(string title)
        {
            try
            {
                Wait.Until(driver => driver.Title != null && driver.Title.Contains(title));
                Log.Information($"Page title verified: {title}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error waiting for page title: {title} - {ex.Message}");
                throw;
            }
        }
    }
}
