using OpenQA.Selenium;
using Serilog;

namespace TakealotAutomation.Pages
{
    /// <summary>
    /// Page object for product details
    /// </summary>
    public class ProductDetailsPage : Core.BasePage
    {
        // Locators
        private readonly By _productTitleLocator = By.XPath("//h1[contains(@class, 'product-title')]");
        private readonly By _productPriceLocator = By.XPath("//span[contains(@class, 'final-price')]");
        private readonly By _productDescriptionLocator = By.XPath("//div[contains(@class, 'product-description')]");
        private readonly By _addToCartButtonLocator = By.XPath("//button[contains(text(), 'Add to Cart')]");
        private readonly By _addToWishlistButtonLocator = By.XPath("//button[contains(text(), 'Add to Wishlist')]");
        private readonly By _quantitySelectorLocator = By.XPath("//input[@type='number' and contains(@class, 'quantity')]");
        private readonly By _ratingLocator = By.XPath("//span[contains(@class, 'rating-score')]");
        private readonly By _reviewsCountLocator = By.XPath("//span[contains(text(), 'Reviews')]");
        private readonly By _stockStatusLocator = By.XPath("//div[contains(@class, 'stock-status')]");

        public ProductDetailsPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Gets product title
        /// </summary>
        public string GetProductTitle()
        {
            string title = GetElementText(_productTitleLocator);
            Log.Information($"Product title: {title}");
            return title;
        }

        /// <summary>
        /// Gets product price
        /// </summary>
        public string GetProductPrice()
        {
            string price = GetElementText(_productPriceLocator);
            Log.Information($"Product price: {price}");
            return price;
        }

        /// <summary>
        /// Gets product rating
        /// </summary>
        public string GetProductRating()
        {
            string rating = GetElementText(_ratingLocator);
            Log.Information($"Product rating: {rating}");
            return rating;
        }

        /// <summary>
        /// Gets stock status
        /// </summary>
        public string GetStockStatus()
        {
            string status = GetElementText(_stockStatusLocator);
            Log.Information($"Stock status: {status}");
            return status;
        }

        /// <summary>
        /// Adds product to cart
        /// </summary>
        public void AddToCart()
        {
            Log.Information("Adding product to cart");
            WaitAndClick(_addToCartButtonLocator);
        }

        /// <summary>
        /// Adds product to wishlist
        /// </summary>
        public void AddToWishlist()
        {
            Log.Information("Adding product to wishlist");
            WaitAndClick(_addToWishlistButtonLocator);
        }

        /// <summary>
        /// Sets product quantity
        /// </summary>
        public void SetQuantity(int quantity)
        {
            Log.Information($"Setting quantity to: {quantity}");
            WaitAndSendKeys(_quantitySelectorLocator, quantity.ToString());
        }

        /// <summary>
        /// Verifies product details page is loaded
        /// </summary>
        public bool IsProductDetailsPageLoaded()
        {
            bool isLoaded = IsElementPresent(_productTitleLocator);
            Log.Information($"Product details page loaded: {isLoaded}");
            return isLoaded;
        }
    }
}
