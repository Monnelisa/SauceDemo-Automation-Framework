using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;

namespace SauceDemoAutomation.Pages
{
    /// <summary>
    /// Page object for Sauce Demo checkout process
    /// </summary>
    public class CheckoutPage : Core.BasePage
    {
        // Locators for Sauce Demo checkout
        private readonly By _firstNameInputLocator = By.Id("first-name");
        private readonly By _lastNameInputLocator = By.Id("last-name");
        private readonly By _postalCodeInputLocator = By.Id("postal-code");
        private readonly By _continueButtonLocator = By.Id("continue");
        private readonly By _finishButtonLocator = By.Id("finish");
        private readonly By _cancelButtonLocator = By.Id("cancel");
        private readonly By _checkoutTitleLocator = By.ClassName("title");
        private readonly By _errorMessageLocator = By.XPath("//h3[@data-test='error']");

        public CheckoutPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Fills in checkout information
        /// </summary>
        public void FillCheckoutInformation(string firstName, string lastName, string postalCode)
        {
            Log.Information("Filling checkout information");
            WaitAndSendKeys(_firstNameInputLocator, firstName);
            WaitAndSendKeys(_lastNameInputLocator, lastName);
            WaitAndSendKeys(_postalCodeInputLocator, postalCode);
        }

        /// <summary>
        /// Continues to next step (cart overview)
        /// </summary>
        public void ContinueToOverview()
        {
            Log.Information("Continuing to overview");
            WaitAndClickWithScroll(_continueButtonLocator);
        }

        /// <summary>
        /// Finishes the order
        /// </summary>
        public OrderConfirmationPage FinishOrder()
        {
            Log.Information("Finishing order");
            WaitAndClickWithScroll(_finishButtonLocator);

            var confirmationPage = new OrderConfirmationPage(Driver);
            if (!confirmationPage.IsOrderConfirmationDisplayed() && IsElementPresent(_finishButtonLocator))
            {
                Log.Warning("Order confirmation not detected after first finish click; retrying finish once");
                WaitAndClickWithScroll(_finishButtonLocator);
                confirmationPage.IsOrderConfirmationDisplayed();
            }

            return confirmationPage;
        }

        /// <summary>
        /// Cancels checkout
        /// </summary>
        public CartPage CancelCheckout()
        {
            Log.Information("Canceling checkout");
            WaitAndClickWithScroll(_cancelButtonLocator);
            return new CartPage(Driver);
        }

        /// <summary>
        /// Gets error message if present
        /// </summary>
        public string GetErrorMessage()
        {
            if (IsElementPresent(_errorMessageLocator))
            {
                return GetElementText(_errorMessageLocator);
            }
            return "";
        }

        /// <summary>
        /// Verifies checkout page is loaded
        /// </summary>
        public bool IsCheckoutPageLoaded()
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
                bool isLoaded = wait.Until(driver =>
                {
                    var firstName = driver.FindElements(_firstNameInputLocator);
                    var lastName = driver.FindElements(_lastNameInputLocator);
                    var postalCode = driver.FindElements(_postalCodeInputLocator);

                    return firstName.Count > 0 && firstName[0].Displayed
                        && lastName.Count > 0 && lastName[0].Displayed
                        && postalCode.Count > 0 && postalCode[0].Displayed;
                });

                Log.Information($"Checkout page loaded: {isLoaded}");
                return isLoaded;
            }
            catch (WebDriverTimeoutException ex)
            {
                Log.Warning($"Checkout page loaded: false - {ex.Message}");
                return false;
            }
        }
    }
}
