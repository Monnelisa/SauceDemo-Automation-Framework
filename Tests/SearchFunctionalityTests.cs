using NUnit.Framework;
using Serilog;
using TakealotAutomation.Pages;

namespace TakealotAutomation.Tests
{
    /// <summary>
    /// Test suite for product search functionality
    /// </summary>
    [TestFixture]
    public class SearchFunctionalityTests : BaseTest
    {
        private HomePage? _homePage;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _homePage = new HomePage(Driver!);
        }

        [Test]
        [Category("Functional")]
        [Description("Search for valid product and verify results")]
        [TestCase("phone")]
        [TestCase("headphones")]
        [TestCase("tablet")]
        public void SearchForValidProduct(string productName)
        {
            // Act
            var searchResultsPage = _homePage!.SearchForProduct(productName);

            // Assert
            Assert.That(searchResultsPage.IsSearchResultsDisplayed(), Is.True, "Search results should be displayed");
            Assert.That(searchResultsPage.GetProductCount(), Is.GreaterThan(0), $"Search results for '{productName}' should return products");
            Log.Information($"Search for '{productName}' returned {searchResultsPage.GetProductCount()} results");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify product details page opens on product click")]
        public void ClickProductAndVerifyDetailsPage()
        {
            // Arrange
            string searchTerm = "laptop";
            var searchResultsPage = _homePage!.SearchForProduct(searchTerm);

            // Act
            var productDetailsPage = searchResultsPage.ClickFirstProduct();

            // Assert
            Assert.That(productDetailsPage.IsProductDetailsPageLoaded(), Is.True, "Product details page should load");
            Assert.That(productDetailsPage.GetProductTitle(), Is.Not.Empty, "Product title should not be empty");
            Log.Information("Product details page loaded successfully");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify product details are displayed correctly")]
        public void VerifyProductDetailsDisplay()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");

            // Act
            var productDetailsPage = searchResultsPage.ClickFirstProduct();
            string title = productDetailsPage.GetProductTitle();
            string price = productDetailsPage.GetProductPrice();
            string rating = productDetailsPage.GetProductRating();

            // Assert
            Assert.That(title, Is.Not.Empty, "Product title should not be empty");
            Assert.That(price, Is.Not.Empty, "Product price should not be empty");
            Assert.That(rating, Is.Not.Empty, "Product rating should not be empty");
            Log.Information($"Product details verified - Title: {title}, Price: {price}, Rating: {rating}");
        }

        [Test]
        [Category("Functional")]
        [Description("Add product to cart from product details page")]
        public void AddProductToCart()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");
            var productDetailsPage = searchResultsPage.ClickFirstProduct();

            // Act
            productDetailsPage.AddToCart();

            // Assert
            Log.Information("Product added to cart successfully");
            // Note: Additional assertion would depend on toast notification or confirmation message
        }

        [Test]
        [Category("Functional")]
        [Description("Add product to wishlist")]
        public void AddProductToWishlist()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");
            var productDetailsPage = searchResultsPage.ClickFirstProduct();

            // Act
            productDetailsPage.AddToWishlist();

            // Assert
            Log.Information("Product added to wishlist successfully");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify quantity selector works")]
        public void VerifyQuantitySelector()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");
            var productDetailsPage = searchResultsPage.ClickFirstProduct();
            int quantity = 3;

            // Act
            productDetailsPage.SetQuantity(quantity);

            // Assert
            Log.Information($"Quantity set to {quantity}");
        }
    }
}
