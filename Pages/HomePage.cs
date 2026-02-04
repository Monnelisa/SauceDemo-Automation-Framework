using OpenQA.Selenium;
using Serilog;

namespace TakealotAutomation.Pages
{
    /// <summary>
    /// Page object for Takealot Homepage
    /// </summary>
    public class HomePage : Core.BasePage
    {
        // Locators
        private readonly By _searchInputLocator = By.XPath("//input[@placeholder='Search for anything']");
        private readonly By _searchButtonLocator = By.XPath("//button[contains(@class, 'search-button')]");
        private readonly By _categoryMenuLocator = By.XPath("//button[contains(text(), 'Categories')]");
        private readonly By _accountIconLocator = By.XPath("//button[contains(@class, 'account')]");
        private readonly By _cartIconLocator = By.XPath("//button[contains(@class, 'basket')]");
        private readonly By _viewAllCategoriesLocator = By.XPath("//a[contains(text(), 'View All')]");

        public HomePage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Searches for a product
        /// </summary>
        public SearchResultsPage SearchForProduct(string productName)
        {
            Log.Information($"Searching for product: {productName}");
            WaitAndSendKeys(_searchInputLocator, productName);
            WaitAndClick(_searchButtonLocator);
            return new SearchResultsPage(Driver);
        }

        /// <summary>
        /// Navigates to cart
        /// </summary>
        public CartPage GoToCart()
        {
            Log.Information("Navigating to cart");
            WaitAndClick(_cartIconLocator);
            return new CartPage(Driver);
        }

        /// <summary>
        /// Opens account menu
        /// </summary>
        public AccountPage GoToAccount()
        {
            Log.Information("Navigating to account");
            WaitAndClick(_accountIconLocator);
            return new AccountPage(Driver);
        }

        /// <summary>
        /// Verifies home page is loaded
        /// </summary>
        public bool IsHomePageLoaded()
        {
            bool isLoaded = IsElementPresent(_searchInputLocator);
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
