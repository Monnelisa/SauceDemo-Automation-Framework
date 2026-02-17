using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;

namespace SauceDemoAutomation.Pages
{
    /// <summary>
    /// Page object for Sauce Demo product details page
    /// </summary>
    public class ProductDetailsPage : Core.BasePage
    {
        // Locators for Sauce Demo product page
        private readonly By _productTitleLocator = By.ClassName("inventory_details_name");
        private readonly By _productPriceLocator = By.ClassName("inventory_details_price");
        private readonly By _productDescriptionLocator = By.ClassName("inventory_details_desc");
        private readonly By _addToCartButtonLocator = By.CssSelector("button[data-test^='add-to-cart']");
        private readonly By _removeButtonLocator = By.CssSelector("button[data-test^='remove']");
        private readonly By _cartBadgeLocator = By.ClassName("shopping_cart_badge");
        private readonly By _backButtonLocator = By.Id("back-to-products");
        private readonly By _productContainerLocator = By.ClassName("inventory_details");

        public ProductDetailsPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Gets product title
        /// </summary>
        public virtual string GetProductTitle()
        {
            string title = GetElementText(_productTitleLocator);
            Log.Information($"Product title: {title}");
            return title;
        }

        /// <summary>
        /// Gets product price
        /// </summary>
        public virtual string GetProductPrice()
        {
            string price = GetElementText(_productPriceLocator);
            Log.Information($"Product price: {price}");
            return price;
        }

        /// <summary>
        /// Gets product description
        /// </summary>
        public string GetProductDescription()
        {
            string description = GetElementText(_productDescriptionLocator);
            Log.Information($"Product description: {description}");
            return description;
        }

        /// <summary>
        /// Adds product to cart
        /// </summary>
        public void AddToCart()
        {
            Log.Information("Adding product to cart");
            WaitForElementToBeVisible(_addToCartButtonLocator);
            WaitAndClickWithScroll(_addToCartButtonLocator);

            // Wait for UI state change to confirm the item was actually added before navigating away.
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            bool added = wait.Until(driver =>
            {
                var removeButtons = driver.FindElements(_removeButtonLocator);
                if (removeButtons.Count > 0 && removeButtons[0].Displayed)
                {
                    return true;
                }

                var cartBadges = driver.FindElements(_cartBadgeLocator);
                if (cartBadges.Count > 0 && int.TryParse(cartBadges[0].Text, out int count) && count > 0)
                {
                    return true;
                }

                return false;
            });

            Log.Information($"Product add confirmed: {added}");
        }

        /// <summary>
        /// Removes product from cart (if the button shows Remove instead of Add)
        /// </summary>
        public void RemoveFromCart()
        {
            Log.Information("Removing product from cart");
            try
            {
                WaitAndClick(_removeButtonLocator);
            }
            catch
            {
                Log.Warning("Remove button not found");
            }
        }

        /// <summary>
        /// Goes back to products page
        /// </summary>
        public HomePage GoBackToProducts()
        {
            Log.Information("Going back to products");
            WaitAndClick(_backButtonLocator);
            return new HomePage(Driver);
        }

        /// <summary>
        /// Verifies product details page is loaded
        /// </summary>
        public virtual bool IsProductDetailsPageLoaded()
        {
            try
            {
                WaitForElementToBeVisible(_productContainerLocator);
                WaitForElementToBeVisible(_productTitleLocator);
                WaitForElementToBeVisible(_productPriceLocator);
                Log.Information("Product details page loaded: true");
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning($"Product details page loaded: false - {ex.Message}");
                return false;
            }
        }
    }
}
