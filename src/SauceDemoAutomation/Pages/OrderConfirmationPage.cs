using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;

namespace SauceDemoAutomation.Pages
{
    /// <summary>
    /// Page object for Sauce Demo order confirmation
    /// </summary>
    public class OrderConfirmationPage : Core.BasePage
    {
        // Locators for Sauce Demo confirmation page
        private readonly By _confirmationMessageLocator = By.ClassName("complete-header");
        private readonly By _confirmationTextLocator = By.ClassName("complete-text");
        private readonly By _backHomeButtonLocator = By.Id("back-to-products");

        public OrderConfirmationPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Gets confirmation message
        /// </summary>
        public string GetConfirmationMessage()
        {
            string message = GetElementText(_confirmationMessageLocator);
            Log.Information($"Confirmation message: {message}");
            return message;
        }

        /// <summary>
        /// Gets detailed confirmation text
        /// </summary>
        public string GetConfirmationText()
        {
            string text = GetElementText(_confirmationTextLocator);
            Log.Information($"Confirmation text: {text}");
            return text;
        }

        /// <summary>
        /// Verifies order confirmation page is displayed
        /// </summary>
        public bool IsOrderConfirmationDisplayed()
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
                bool isDisplayed = wait.Until(driver =>
                {
                    var confirmations = driver.FindElements(_confirmationMessageLocator);
                    return confirmations.Count > 0 && confirmations[0].Displayed;
                });

                Log.Information($"Order confirmation displayed: {isDisplayed}");
                return isDisplayed;
            }
            catch (WebDriverTimeoutException ex)
            {
                Log.Warning($"Order confirmation displayed: false - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Goes back to home/products page
        /// </summary>
        public HomePage BackToHome()
        {
            Log.Information("Going back home");
            WaitAndClick(_backHomeButtonLocator);
            return new HomePage(Driver);
        }
    }
}
