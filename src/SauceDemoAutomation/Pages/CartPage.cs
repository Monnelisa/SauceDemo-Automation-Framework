using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Collections.Generic;

namespace SauceDemoAutomation.Pages
{
    /// <summary>
    /// Page object for Sauce Demo shopping cart
    /// </summary>
    public class CartPage : Core.BasePage
    {
        // Locators for Sauce Demo cart page
        private readonly By _cartItemsLocator = By.ClassName("cart_item");
        private readonly By _cartItemNameLocator = By.ClassName("inventory_item_name");
        private readonly By _cartItemPriceLocator = By.ClassName("inventory_item_price");
        private readonly By _removeButtonLocator = By.XPath(".//button[contains(@class, 'btn_secondary')]");
        private readonly By _checkoutButtonLocator = By.Id("checkout");
        private readonly By _continueShoppingButtonLocator = By.Id("continue-shopping");
        private readonly By _cartTitleLocator = By.ClassName("title");
        private readonly By _cartContainerLocator = By.ClassName("cart_list");

        public CartPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Gets number of items in cart
        /// </summary>
        public int GetCartItemCount()
        {
            WaitForElementToBeVisible(_cartContainerLocator);
            var items = Driver.FindElements(_cartItemsLocator);
            int count = items.Count;
            Log.Information($"Cart item count: {count}");
            return count;
        }

        /// <summary>
        /// Gets list of cart items
        /// </summary>
        public IList<IWebElement> GetCartItems()
        {
            WaitForElementToBeVisible(_cartContainerLocator);
            var items = Driver.FindElements(_cartItemsLocator);
            Log.Information($"Retrieved {items.Count} cart items");
            return items;
        }

        /// <summary>
        /// Removes item at specified index
        /// </summary>
        public void RemoveItemAtIndex(int index)
        {
            Log.Information($"Removing item at index: {index}");
            var items = GetCartItems();
            if (items.Count > index)
            {
                var removeButton = items[index].FindElement(_removeButtonLocator);
                try
                {
                    removeButton.Click();
                }
                catch (Exception ex) when (ex is ElementClickInterceptedException || ex is WebDriverException || ex is StaleElementReferenceException)
                {
                    IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)Driver;
                    jsExecutor.ExecuteScript("arguments[0].click();", removeButton);
                }
            }
        }

        /// <summary>
        /// Proceeds to checkout
        /// </summary>
        public CheckoutPage ProceedToCheckout()
        {
            Log.Information("Proceeding to checkout");
            WaitAndClickWithScroll(_checkoutButtonLocator);
            return new CheckoutPage(Driver);
        }

        /// <summary>
        /// Continues shopping (goes back to inventory)
        /// </summary>
        public HomePage ContinueShopping()
        {
            Log.Information("Continuing shopping");
            WaitForElementToBeVisible(_continueShoppingButtonLocator);
            WaitAndClickWithScroll(_continueShoppingButtonLocator);
            return new HomePage(Driver);
        }

        /// <summary>
        /// Verifies cart page is loaded
        /// </summary>
        public bool IsCartPageLoaded()
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
                bool isLoaded = wait.Until(driver =>
                {
                    bool onCartUrl = driver.Url.Contains("cart", StringComparison.OrdinalIgnoreCase);
                    var checkoutButtons = driver.FindElements(_checkoutButtonLocator);
                    var continueButtons = driver.FindElements(_continueShoppingButtonLocator);

                    bool cartActionsVisible =
                        (checkoutButtons.Count > 0 && checkoutButtons[0].Displayed) ||
                        (continueButtons.Count > 0 && continueButtons[0].Displayed);

                    return onCartUrl && cartActionsVisible;
                });

                Log.Information($"Cart page loaded: {isLoaded}");
                return isLoaded;
            }
            catch (WebDriverTimeoutException ex)
            {
                Log.Warning($"Cart page loaded: false - {ex.Message}");
                return false;
            }
        }
    }
}
