using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Collections.Generic;

namespace SauceDemoAutomation.Pages
{
    /// <summary>
    /// Page object for search results
    /// </summary>
    public class SearchResultsPage : Core.BasePage
    {
        // Locators
        private readonly By _productItemsLocator = By.XPath("//div[contains(@class, 'product-item')]");
        private readonly By _productPriceLocator = By.XPath(".//span[contains(@class, 'price')]");
        private readonly By _productNameLocator = By.XPath(".//a[contains(@class, 'product-name')]");
        private readonly By _sortDropdownLocator = By.XPath("//select[contains(@class, 'sort')]");
        private readonly By _filterButtonLocator = By.XPath("//button[contains(text(), 'Filters')]");
        private readonly By _searchResultsHeaderLocator = By.XPath("//h1[contains(text(), 'Search Results')]");
        private readonly By _noResultsMessageLocator = By.XPath("//p[contains(text(), 'No products found')]");

        public SearchResultsPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Gets list of products displayed
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
        /// Clicks on first product
        /// </summary>
        public ProductDetailsPage ClickFirstProduct()
        {
            Log.Information("Clicking first product");
            WaitAndClick(_productItemsLocator);
            return new ProductDetailsPage(Driver);
        }

        /// <summary>
        /// Filters results by price range
        /// </summary>
        public void FilterByPrice(decimal minPrice, decimal maxPrice)
        {
            Log.Information($"Filtering by price: {minPrice} - {maxPrice}");
            WaitAndClick(_filterButtonLocator);
            // Add specific price filter logic here based on actual website structure
        }

        /// <summary>
        /// Verifies search results are displayed
        /// </summary>
        public bool AreSearchResultsDisplayed()
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                bool resultsVisible = wait.Until(driver =>
                {
                    var items = driver.FindElements(_productItemsLocator);
                    return items.Count > 0 && items[0].Displayed;
                });

                Log.Information($"Search results displayed: {resultsVisible}");
                return resultsVisible;
            }
            catch (WebDriverTimeoutException ex)
            {
                Log.Warning($"Search results displayed: false - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Alias used by tests for clarity
        /// </summary>
        public bool IsSearchResultsDisplayed() => AreSearchResultsDisplayed();

        /// <summary>
        /// Alias to indicate the page is loaded; checks header or results
        /// </summary>
        public bool IsSearchResultsPageLoaded()
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
                bool loaded = wait.Until(driver =>
                {
                    var headers = driver.FindElements(_searchResultsHeaderLocator);
                    bool headerPresent = headers.Count > 0 && headers[0].Displayed;

                    var items = driver.FindElements(_productItemsLocator);
                    bool resultsPresent = items.Count > 0 && items[0].Displayed;

                    return headerPresent || resultsPresent;
                });

                Log.Information($"Search results page loaded: {loaded}");
                return loaded;
            }
            catch (WebDriverTimeoutException ex)
            {
                Log.Warning($"Search results page loaded: false - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if no results message is shown
        /// </summary>
        public bool IsNoResultsMessageDisplayed()
        {
            return IsElementPresent(_noResultsMessageLocator);
        }
    }
}
