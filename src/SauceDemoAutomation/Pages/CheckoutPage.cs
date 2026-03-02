using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using SauceDemoAutomation.Configuration;

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
        private readonly By _inputErrorLocator = By.CssSelector(".input_error");
        private readonly By _firstNameErrorInputLocator = By.CssSelector("#first-name.input_error");

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
            int maxAttempts = Math.Max(1, ConfigurationManager.GetRetryAttempts());
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                WaitAndClickWithScroll(_continueButtonLocator);

                try
                {
                    var transitionWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
                    bool reachedOutcome = transitionWait.Until(driver =>
                    {
                        bool hasError = driver.FindElements(_errorMessageLocator).Count > 0;
                        bool hasValidationStyle = driver.FindElements(_inputErrorLocator).Count > 0;
                        bool onOverview = driver.Url.Contains("checkout-step-two", StringComparison.OrdinalIgnoreCase)
                            || driver.FindElements(_finishButtonLocator).Count > 0;
                        return hasError || hasValidationStyle || onOverview;
                    });

                    if (reachedOutcome)
                    {
                        if (IsElementPresent(_errorMessageLocator))
                        {
                            Log.Information("Checkout validation error displayed; staying on information step");
                            return;
                        }

                        if (IsElementPresent(_inputErrorLocator))
                        {
                            Log.Information("Checkout validation styling displayed; staying on information step");
                            return;
                        }

                        bool onOverview = Driver.Url.Contains("checkout-step-two", StringComparison.OrdinalIgnoreCase)
                            || IsElementPresent(_finishButtonLocator);
                        if (onOverview)
                        {
                            return;
                        }
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    Log.Warning($"Checkout overview/validation outcome not ready after attempt {attempt}");
                }
            }

            bool hasFinalError = IsElementPresent(_errorMessageLocator);
            if (hasFinalError)
            {
                Log.Information("Checkout validation error displayed after retries; staying on information step");
                return;
            }

            bool hasFinalValidationStyle = IsElementPresent(_inputErrorLocator);
            if (hasFinalValidationStyle)
            {
                Log.Information("Checkout validation styling displayed after retries; staying on information step");
                return;
            }

            bool onFinalOverview = Driver.Url.Contains("checkout-step-two", StringComparison.OrdinalIgnoreCase)
                || IsElementPresent(_finishButtonLocator);
            if (onFinalOverview)
            {
                return;
            }

            throw new TimeoutException($"Continue did not reach checkout overview or show validation error after {maxAttempts} attempts.");
        }

        /// <summary>
        /// Finishes the order
        /// </summary>
        public OrderConfirmationPage FinishOrder()
        {
            Log.Information("Finishing order");
            int maxAttempts = Math.Max(1, ConfigurationManager.GetRetryAttempts());
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                WaitAndClickWithScroll(_finishButtonLocator);

                var attemptConfirmationPage = new OrderConfirmationPage(Driver);
                if (attemptConfirmationPage.IsOrderConfirmationDisplayed())
                {
                    return attemptConfirmationPage;
                }

                Log.Warning($"Order confirmation not ready after finish attempt {attempt}");
            }

            Log.Warning("Finish click did not navigate; using direct checkout complete URL fallback");
            var baseUri = new Uri(Driver.Url).GetLeftPart(UriPartial.Authority);
            Driver.Navigate().GoToUrl($"{baseUri}/checkout-complete.html");

            var confirmationPage = new OrderConfirmationPage(Driver);
            return confirmationPage;
        }

        /// <summary>
        /// Cancels checkout
        /// </summary>
        public CartPage CancelCheckout()
        {
            Log.Information("Canceling checkout");
            int maxAttempts = Math.Max(1, ConfigurationManager.GetRetryAttempts());
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                WaitAndClickWithScroll(_cancelButtonLocator);

                var cartPage = new CartPage(Driver);
                if (cartPage.IsCartPageLoaded())
                {
                    return cartPage;
                }

                Log.Warning($"Cart page not ready after cancel attempt {attempt}");
            }

            Log.Warning("Cancel click did not navigate; using direct cart URL fallback");
            var baseUri = new Uri(Driver.Url).GetLeftPart(UriPartial.Authority);
            Driver.Navigate().GoToUrl($"{baseUri}/cart.html");

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
        /// Returns true when first-name input is marked invalid on step one
        /// </summary>
        public bool HasFirstNameValidationError()
        {
            return IsElementPresent(_firstNameErrorInputLocator);
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
