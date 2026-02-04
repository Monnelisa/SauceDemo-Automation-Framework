using OpenQA.Selenium;
using Serilog;

namespace TakealotAutomation.Pages
{
    /// <summary>
    /// Page object for shopping cart
    /// </summary>
    public class CartPage : Core.BasePage
    {
        // Locators
        private readonly By _cartItemsLocator = By.XPath("//div[contains(@class, 'cart-item')]");
        private readonly By _cartTotalLocator = By.XPath("//span[contains(@class, 'cart-total')]");
        private readonly By _checkoutButtonLocator = By.XPath("//button[contains(text(), 'Checkout')]");
        private readonly By _removeItemButtonLocator = By.XPath(".//button[contains(text(), 'Remove')]");
        private readonly By _emptyCartMessageLocator = By.XPath("//p[contains(text(), 'Your cart is empty')]");
        private readonly By _continueShoppingButtonLocator = By.XPath("//button[contains(text(), 'Continue Shopping')]");
        private readonly By _promoCodeInputLocator = By.XPath("//input[contains(@placeholder, 'Promo Code')]");
        private readonly By _applyPromoButtonLocator = By.XPath("//button[contains(text(), 'Apply')]");

        public CartPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Gets number of items in cart
        /// </summary>
        public int GetCartItemCount()
        {
            int count = Driver.FindElements(_cartItemsLocator).Count;
            Log.Information($"Cart item count: {count}");
            return count;
        }

        /// <summary>
        /// Gets cart total
        /// </summary>
        public string GetCartTotal()
        {
            string total = GetElementText(_cartTotalLocator);
            Log.Information($"Cart total: {total}");
            return total;
        }

        /// <summary>
        /// Proceeds to checkout
        /// </summary>
        public CheckoutPage ProceedToCheckout()
        {
            Log.Information("Proceeding to checkout");
            WaitAndClick(_checkoutButtonLocator);
            return new CheckoutPage(Driver);
        }

        /// <summary>
        /// Removes an item from cart
        /// </summary>
        public void RemoveItemFromCart(int itemIndex)
        {
            Log.Information($"Removing item at index: {itemIndex}");
            var items = Driver.FindElements(_cartItemsLocator);
            if (items.Count > itemIndex)
            {
                var removeButton = items[itemIndex].FindElement(_removeItemButtonLocator);
                removeButton.Click();
            }
        }

        /// <summary>
        /// Applies promo code
        /// </summary>
        public void ApplyPromoCode(string promoCode)
        {
            Log.Information($"Applying promo code: {promoCode}");
            WaitAndSendKeys(_promoCodeInputLocator, promoCode);
            WaitAndClick(_applyPromoButtonLocator);
        }

        /// <summary>
        /// Checks if cart is empty
        /// </summary>
        public bool IsCartEmpty()
        {
            bool isEmpty = IsElementPresent(_emptyCartMessageLocator);
            Log.Information($"Cart is empty: {isEmpty}");
            return isEmpty;
        }

        /// <summary>
        /// Verifies cart page is loaded
        /// </summary>
        public bool IsCartPageLoaded()
        {
            bool isLoaded = IsElementPresent(_cartTotalLocator) || IsElementPresent(_emptyCartMessageLocator);
            Log.Information($"Cart page loaded: {isLoaded}");
            return isLoaded;
        }
    }
}
