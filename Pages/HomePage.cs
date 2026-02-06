using OpenQA.Selenium;
using Serilog;
using System.Collections.Generic;

namespace TakealotAutomation.Pages
{
    /// <summary>
    /// Page object for Sauce Demo Inventory (Products) page
    /// </summary>
    public class HomePage : Core.BasePage
    {
        // Locators for Sauce Demo inventory page
        private readonly By _productItemsLocator = By.ClassName("inventory_item");
        private readonly By _productNameLocator = By.ClassName("inventory_item_name");
        private readonly By _productPriceLocator = By.ClassName("inventory_item_price");
        private readonly By _addToCartButtonLocator = By.XPath("//button[contains(@class, 'btn_primary')]");
        private readonly By _cartLinkLocator = By.ClassName("shopping_cart_link");
        private readonly By _cartBadgeLocator = By.ClassName("shopping_cart_badge");
        private readonly By _sortDropdownLocator = By.ClassName("product_sort_container");
        private readonly By _menuButtonLocator = By.Id("react-burger-menu-btn");
        private readonly By _logoutLinkLocator = By.Id("logout_sidebar_link");
        private readonly By _titleLocator = By.ClassName("title");

        public HomePage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Gets list of product items displayed
        /// </summary>
        public IList<IWebElement> GetProductItems()
        {
            var items = Driver.FindElements(_productItemsLocator);
            Log.Information($"Found {items.Count} products");
            return items;
        }

        /// <summary>
        /// Gets product count
        /// </summary>
        public int GetProductCount()
        {
            int count = GetProductItems().Count;
            Log.Information($"Product count: {count}");
            return count;
        }

        /// <summary>
        /// Clicks on first product to view details
        /// </summary>
        public ProductDetailsPage ClickFirstProduct()
        {
            Log.Information("Clicking first product");
            WaitAndClick(_productNameLocator);
            return new ProductDetailsPage(Driver);
        }

        /// <summary>
        /// Clicks product by index
        /// </summary>
        public ProductDetailsPage ClickProductByIndex(int index)
        {
            Log.Information($"Clicking product at index: {index}");
            var products = GetProductItems();
            if (products.Count > index)
            {
                products[index].FindElement(_productNameLocator).Click();
            }
            return new ProductDetailsPage(Driver);
        }

        /// <summary>
        /// Adds first product to cart
        /// </summary>
        public void AddFirstProductToCart()
        {
            Log.Information("Adding first product to cart");
            var products = GetProductItems();
            if (products.Count > 0)
            {
                var addButton = products[0].FindElement(_addToCartButtonLocator);
                addButton.Click();
            }
        }

        /// <summary>
        /// Navigates to cart
        /// </summary>
        public CartPage GoToCart()
        {
            Log.Information("Navigating to cart");
            WaitAndClick(_cartLinkLocator);
            return new CartPage(Driver);
        }

        /// <summary>
        /// Gets cart badge count
        /// </summary>
        public int GetCartItemCount()
        {
            try
            {
                string badgeText = GetElementText(_cartBadgeLocator);
                if (int.TryParse(badgeText, out int count))
                {
                    return count;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Sorts products by option
        /// </summary>
        public void SortBy(string sortOption)
        {
            Log.Information($"Sorting by: {sortOption}");
            WaitAndClick(_sortDropdownLocator);
            var option = Driver.FindElement(By.XPath($"//option[contains(text(), '{sortOption}')]"));
            option.Click();
        }

        /// <summary>
        /// Opens menu and logs out
        /// </summary>
        public LoginPage Logout()
        {
            Log.Information("Logging out");
            WaitAndClick(_menuButtonLocator);
            WaitAndClick(_logoutLinkLocator);
            return new LoginPage(Driver);
        }

        /// <summary>
        /// Verifies home page is loaded
        /// </summary>
        public bool IsHomePageLoaded()
        {
            bool isLoaded = IsElementPresent(_productItemsLocator);
            Log.Information($"Home page loaded: {isLoaded}");
            return isLoaded;
        }

        /// <summary>
        /// Gets page title
        /// </summary>
        public string GetPageTitle()
        {
            string title = Driver.Title;
            Log.Information($"Page title: {title}");
            return title;
        }
    }
}
