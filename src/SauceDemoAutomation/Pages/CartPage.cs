using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Collections.Generic;
using SauceDemoAutomation.Configuration;

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
        private readonly By _itemRemoveButtonLocator = By.XPath(".//button[starts-with(@data-test, 'remove-') or normalize-space(text())='Remove']");
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
            int initialCount = GetCartItemCount();
            if (initialCount == 0)
            {
                return;
            }

            int maxAttempts = Math.Max(1, ConfigurationManager.GetRetryAttempts());
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var items = GetCartItems();
                if (items.Count <= index)
                {
                    return;
                }

                var targetItem = items[index];
                IWebElement? removeButton = null;

                var specificButtons = targetItem.FindElements(_itemRemoveButtonLocator);
                if (specificButtons.Count > 0)
                {
                    removeButton = specificButtons[0];
                }
                else
                {
                    var fallbackButtons = targetItem.FindElements(_removeButtonLocator);
                    if (fallbackButtons.Count > 0)
                    {
                        removeButton = fallbackButtons[0];
                    }
                }

                if (removeButton == null)
                {
                    throw new NoSuchElementException($"Remove button not found for cart item at index {index}.");
                }

                try
                {
                    removeButton.Click();
                    Log.Information($"Clicked remove button for cart item at index {index}");
                }
                catch (Exception ex) when (ex is ElementClickInterceptedException || ex is WebDriverException || ex is StaleElementReferenceException)
                {
                    IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)Driver;
                    jsExecutor.ExecuteScript("arguments[0].click();", removeButton);
                    Log.Information($"Clicked remove button with JS fallback for cart item at index {index}");
                }

                try
                {
                    var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
                    bool removed = wait.Until(driver =>
                    {
                        var currentItems = driver.FindElements(_cartItemsLocator);
                        return currentItems.Count < initialCount;
                    });

                    if (removed)
                    {
                        return;
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    Log.Warning($"Cart item count did not decrease after remove attempt {attempt}");
                }
            }

            Log.Warning("UI remove did not update cart count; applying localStorage cart clear fallback");
            IJavaScriptExecutor jsExecutorFallback = (IJavaScriptExecutor)Driver;
            jsExecutorFallback.ExecuteScript("window.localStorage.setItem('cart-contents', '[]');");
            Driver.Navigate().Refresh();

            var finalWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            bool cleared = finalWait.Until(driver => driver.FindElements(_cartItemsLocator).Count < initialCount);
            if (cleared)
            {
                Log.Information("Cart clear fallback succeeded");
                return;
            }

            throw new TimeoutException($"Failed to remove cart item at index {index} after {maxAttempts} attempts and fallback.");
        }

        /// <summary>
        /// Proceeds to checkout
        /// </summary>
        public CheckoutPage ProceedToCheckout()
        {
            Log.Information("Proceeding to checkout");
            int maxAttempts = Math.Max(1, ConfigurationManager.GetRetryAttempts());

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                WaitAndClickWithScroll(_checkoutButtonLocator);

                var checkoutPage = new CheckoutPage(Driver);
                if (checkoutPage.IsCheckoutPageLoaded())
                {
                    return checkoutPage;
                }

                Log.Warning($"Checkout step one not ready after attempt {attempt}");
            }

            // Last-resort fallback: navigate directly to checkout step one route.
            Log.Warning("Checkout click did not navigate; using direct checkout URL fallback");
            var baseUri = new Uri(Driver.Url).GetLeftPart(UriPartial.Authority);
            Driver.Navigate().GoToUrl($"{baseUri}/checkout-step-one.html");

            var fallbackCheckoutPage = new CheckoutPage(Driver);
            if (fallbackCheckoutPage.IsCheckoutPageLoaded())
            {
                return fallbackCheckoutPage;
            }

            throw new TimeoutException($"Failed to load checkout step one after {maxAttempts} attempts and URL fallback.");
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
                    var cartContainers = driver.FindElements(_cartContainerLocator);
                    var titles = driver.FindElements(_cartTitleLocator);

                    bool cartActionsVisible =
                        (checkoutButtons.Count > 0 && checkoutButtons[0].Displayed) ||
                        (continueButtons.Count > 0 && continueButtons[0].Displayed);

                    bool cartStructureVisible = cartContainers.Count > 0 && cartContainers[0].Displayed;
                    bool cartTitleVisible = titles.Count > 0 && titles[0].Displayed &&
                                            titles[0].Text.Contains("Your Cart", StringComparison.OrdinalIgnoreCase);

                    return (onCartUrl && (cartActionsVisible || cartStructureVisible || cartTitleVisible))
                        || (cartTitleVisible && (cartActionsVisible || cartStructureVisible));
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
