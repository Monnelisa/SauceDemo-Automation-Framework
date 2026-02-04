using OpenQA.Selenium;
using Serilog;

namespace TakealotAutomation.Pages
{
    /// <summary>
    /// Page object for order confirmation
    /// </summary>
    public class OrderConfirmationPage : Core.BasePage
    {
        // Locators
        private readonly By _orderNumberLocator = By.XPath("//span[contains(text(), 'Order Number:')]/following-sibling::span");
        private readonly By _confirmationMessageLocator = By.XPath("//h1[contains(text(), 'Thank You')]");
        private readonly By _orderDetailsLocator = By.XPath("//div[contains(@class, 'order-details')]");
        private readonly By _continueShoppingButtonLocator = By.XPath("//button[contains(text(), 'Continue Shopping')]");

        public OrderConfirmationPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Gets order number
        /// </summary>
        public string GetOrderNumber()
        {
            string orderNumber = GetElementText(_orderNumberLocator);
            Log.Information($"Order number: {orderNumber}");
            return orderNumber;
        }

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
        /// Verifies order confirmation page is displayed
        /// </summary>
        public bool IsOrderConfirmationDisplayed()
        {
            bool isDisplayed = IsElementPresent(_confirmationMessageLocator) && IsElementPresent(_orderNumberLocator);
            Log.Information($"Order confirmation displayed: {isDisplayed}");
            return isDisplayed;
        }

        /// <summary>
        /// Continues shopping
        /// </summary>
        public HomePage ContinueShopping()
        {
            Log.Information("Continuing shopping");
            WaitAndClick(_continueShoppingButtonLocator);
            return new HomePage(Driver);
        }
    }
}
