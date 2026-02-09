using OpenQA.Selenium;
using Serilog;
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
                removeButton.Click();
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
            WaitAndClick(_continueShoppingButtonLocator);
            return new HomePage(Driver);
        }

        /// <summary>
        /// Verifies cart page is loaded
        /// </summary>
        public bool IsCartPageLoaded()
        {
            bool isLoaded = IsElementPresent(_checkoutButtonLocator) || IsElementPresent(_continueShoppingButtonLocator);
            Log.Information($"Cart page loaded: {isLoaded}");
            return isLoaded;
        }
    }
}
