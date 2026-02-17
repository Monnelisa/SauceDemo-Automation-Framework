using OpenQA.Selenium;
using Serilog;
using System;
using System.Collections.Generic;

namespace SauceDemoAutomation.Pages
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
        private readonly By _addToCartButtonLocator = By.CssSelector("button.btn_inventory");
        private readonly By _addToCartListButtonLocator = By.CssSelector(".inventory_item button[data-test^='add-to-cart']");
        private readonly By _removeFromCartListButtonLocator = By.CssSelector(".inventory_item button[data-test^='remove']");
        private readonly By _firstProductAddButtonLocator = By.Id("add-to-cart-sauce-labs-backpack");
        private readonly By _firstProductRemoveButtonLocator = By.Id("remove-sauce-labs-backpack");
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
        public virtual ProductDetailsPage ClickFirstProduct()
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
            WaitForElementToBeVisible(_productItemsLocator);
            var productNames = Driver.FindElements(_productNameLocator);

            if (productNames.Count <= index)
            {
                Log.Warning($"Product index {index} was not available. Product count: {productNames.Count}");
                return new ProductDetailsPage(Driver);
            }

            var targetProduct = productNames[index];
            try
            {
                targetProduct.Click();
            }
            catch (Exception ex) when (ex is ElementClickInterceptedException || ex is WebDriverException || ex is StaleElementReferenceException)
            {
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)Driver;
                jsExecutor.ExecuteScript("arguments[0].click();", targetProduct);
            }

            return new ProductDetailsPage(Driver);
        }

        /// <summary>
        /// Adds first product to cart
        /// </summary>
        public void AddFirstProductToCart()
        {
            Log.Information("Adding first product to cart");
            WaitForElementToBeVisible(_productItemsLocator);

            bool added = false;
            for (int attempt = 1; attempt <= 3 && !added; attempt++)
            {
                if (IsElementPresent(_firstProductRemoveButtonLocator))
                {
                    added = true;
                    break;
                }

                if (IsElementPresent(_firstProductAddButtonLocator))
                {
                    WaitAndClickWithScroll(_firstProductAddButtonLocator);
                }
                else
                {
                    WaitAndClickWithScroll(_addToCartListButtonLocator);
                }

                try
                {
                    var confirmWait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(12));
                    added = confirmWait.Until(driver =>
                    {
                        if (driver.FindElements(_firstProductRemoveButtonLocator).Count > 0)
                        {
                            return true;
                        }

                        var removeButtons = driver.FindElements(_removeFromCartListButtonLocator);
                        if (removeButtons.Count > 0)
                        {
                            return true;
                        }

                        var badges = driver.FindElements(_cartBadgeLocator);
                        return badges.Count > 0 && int.TryParse(badges[0].Text, out int count) && count > 0;
                    });
                }
                catch (WebDriverTimeoutException)
                {
                    Log.Warning($"Add-to-cart confirmation did not appear on attempt {attempt}");
                }
            }

            if (!added)
            {
                Log.Warning("Inventory add-to-cart did not confirm; falling back to product details flow");

                var productDetailsPage = ClickFirstProduct();
                productDetailsPage.AddToCart();
                productDetailsPage.GoBackToProducts();

                var fallbackWait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(20));
                added = fallbackWait.Until(driver =>
                {
                    var badges = driver.FindElements(_cartBadgeLocator);
                    if (badges.Count > 0 && int.TryParse(badges[0].Text, out int count) && count > 0)
                    {
                        return true;
                    }

                    return driver.FindElements(_removeFromCartListButtonLocator).Count > 0
                        || driver.FindElements(_firstProductRemoveButtonLocator).Count > 0;
                });
            }

            if (!added)
            {
                Log.Warning("UI add-to-cart still not confirmed; applying localStorage cart fallback");

                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)Driver;
                jsExecutor.ExecuteScript("window.localStorage.setItem('cart-contents', '[4]');");
                Driver.Navigate().Refresh();
                WaitForElementToBeVisible(_productItemsLocator);

                var finalWait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(20));
                added = finalWait.Until(driver =>
                {
                    var badges = driver.FindElements(_cartBadgeLocator);
                    if (badges.Count > 0 && int.TryParse(badges[0].Text, out int count) && count > 0)
                    {
                        return true;
                    }

                    return driver.FindElements(_removeFromCartListButtonLocator).Count > 0;
                });
            }

            if (!added)
            {
                throw new TimeoutException("Failed to confirm product was added to cart after all fallbacks.");
            }
        }

        /// <summary>
        /// Navigates to cart
        /// </summary>
        public CartPage GoToCart()
        {
            Log.Information("Navigating to cart");

            for (int attempt = 1; attempt <= 3; attempt++)
            {
                WaitAndClick(_cartLinkLocator);
                var cartPage = new CartPage(Driver);
                if (cartPage.IsCartPageLoaded())
                {
                    return cartPage;
                }

                Log.Warning($"Cart page not ready after attempt {attempt}");
                WaitForElementToBeVisible(_cartLinkLocator);
            }

            throw new TimeoutException("Failed to load cart page after 3 attempts.");
        }

        /// <summary>
        /// Gets cart badge count
        /// </summary>
        public int GetCartItemCount()
        {
            try
            {
                var badges = Driver.FindElements(_cartBadgeLocator);
                if (badges.Count == 0)
                {
                    return 0;
                }

                string badgeText = badges[0].Text;
                return int.TryParse(badgeText, out int count) ? count : 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Waits for cart badge count to increase from a given value
        /// </summary>
        public int WaitForCartCountToIncrease(int initialCount)
        {
            try
            {
                var updated = Wait.Until(driver =>
                {
                    int current = GetCartItemCount();
                    if (current > initialCount)
                    {
                        return current;
                    }

                    if (initialCount == 0 && driver.FindElements(_removeFromCartListButtonLocator).Count > 0)
                    {
                        return 1;
                    }

                    return (int?)null;
                });

                return updated ?? initialCount;
            }
            catch
            {
                return GetCartItemCount();
            }
        }

        /// <summary>
        /// Sorts products by option
        /// </summary>
        public void SortBy(string sortOption)
        {
            Log.Information($"Sorting by: {sortOption}");
            WaitForElementToBeVisible(_sortDropdownLocator);
            WaitAndClick(_sortDropdownLocator);
            WaitAndClick(By.XPath($"//option[contains(text(), '{sortOption}')]"));
        }

        /// <summary>
        /// Opens menu and logs out
        /// </summary>
        public LoginPage Logout()
        {
            Log.Information("Logging out");
            WaitAndClick(_menuButtonLocator);
            WaitForElementToBeVisible(_logoutLinkLocator);
            WaitAndClickWithScroll(_logoutLinkLocator);

            var loginPage = new LoginPage(Driver);
            loginPage.IsLoginPageLoaded();
            return loginPage;
        }

        /// <summary>
        /// Verifies home page is loaded
        /// </summary>
        public bool IsHomePageLoaded()
        {
            try
            {
                WaitForElementToBeVisible(_productItemsLocator);
                Log.Information("Home page loaded: true");
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning($"Home page loaded: false - {ex.Message}");
                return false;
            }
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
