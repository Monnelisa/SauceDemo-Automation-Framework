using OpenQA.Selenium;
using Serilog;

namespace TakealotAutomation.Pages
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
            try
            {
                string message = GetElementText(_confirmationMessageLocator);
                Log.Information($"Confirmation message: {message}");
                return message;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Gets detailed confirmation text
        /// </summary>
        public string GetConfirmationText()
        {
            try
            {
                string text = GetElementText(_confirmationTextLocator);
                Log.Information($"Confirmation text: {text}");
                return text;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Verifies order confirmation page is displayed
        /// </summary>
        public bool IsOrderConfirmationDisplayed()
        {
            bool isDisplayed = IsElementPresent(_confirmationMessageLocator);
            Log.Information($"Order confirmation displayed: {isDisplayed}");
            return isDisplayed;
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
