using OpenQA.Selenium;
using Serilog;

namespace TakealotAutomation.Pages
{
    /// <summary>
    /// Page object for checkout process
    /// </summary>
    public class CheckoutPage : Core.BasePage
    {
        // Locators
        private readonly By _emailInputLocator = By.XPath("//input[@type='email']");
        private readonly By _firstNameInputLocator = By.XPath("//input[@placeholder='First Name']");
        private readonly By _lastNameInputLocator = By.XPath("//input[@placeholder='Last Name']");
        private readonly By _addressInputLocator = By.XPath("//input[@placeholder='Address']");
        private readonly By _cityInputLocator = By.XPath("//input[@placeholder='City']");
        private readonly By _postalCodeInputLocator = By.XPath("//input[@placeholder='Postal Code']");
        private readonly By _phoneInputLocator = By.XPath("//input[@type='tel']");
        private readonly By _continueButtonLocator = By.XPath("//button[contains(text(), 'Continue')]");
        private readonly By _placeOrderButtonLocator = By.XPath("//button[contains(text(), 'Place Order')]");
        private readonly By _orderSummaryLocator = By.XPath("//div[contains(@class, 'order-summary')]");

        public CheckoutPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Fills in customer information
        /// </summary>
        public void FillCustomerInformation(string email, string firstName, string lastName, string phone)
        {
            Log.Information("Filling customer information");
            WaitAndSendKeys(_emailInputLocator, email);
            WaitAndSendKeys(_firstNameInputLocator, firstName);
            WaitAndSendKeys(_lastNameInputLocator, lastName);
            WaitAndSendKeys(_phoneInputLocator, phone);
        }

        /// <summary>
        /// Fills in delivery address
        /// </summary>
        public void FillDeliveryAddress(string address, string city, string postalCode)
        {
            Log.Information("Filling delivery address");
            WaitAndSendKeys(_addressInputLocator, address);
            WaitAndSendKeys(_cityInputLocator, city);
            WaitAndSendKeys(_postalCodeInputLocator, postalCode);
        }

        /// <summary>
        /// Proceeds to payment step
        /// </summary>
        public void ContinueToPayment()
        {
            Log.Information("Continuing to payment");
            WaitAndClick(_continueButtonLocator);
        }

        /// <summary>
        /// Places order
        /// </summary>
        public OrderConfirmationPage PlaceOrder()
        {
            Log.Information("Placing order");
            WaitAndClick(_placeOrderButtonLocator);
            return new OrderConfirmationPage(Driver);
        }

        /// <summary>
        /// Verifies checkout page is loaded
        /// </summary>
        public bool IsCheckoutPageLoaded()
        {
            bool isLoaded = IsElementPresent(_emailInputLocator);
            Log.Information($"Checkout page loaded: {isLoaded}");
            return isLoaded;
        }
    }
}
