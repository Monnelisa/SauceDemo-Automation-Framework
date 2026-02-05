using NUnit.Framework;
using Serilog;
using TakealotAutomation.Pages;

namespace TakealotAutomation.Tests
{
    /// <summary>
    /// Test suite for Sauce Demo product browsing and filtering
    /// </summary>
    [TestFixture]
    public class SearchFunctionalityTests : BaseTest
    {
        private LoginPage? _loginPage;
        private HomePage? _homePage;
        private const string ValidUsername = "standard_user";
        private const string ValidPassword = "secret_sauce";

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _loginPage = new LoginPage(Driver!);
            _homePage = _loginPage.Login(ValidUsername, ValidPassword);
        }

        [Test]
        [Category("Functional")]
        [Description("Verify all products are displayed")]
        public void VerifyAllProductsDisplayed()
        {
            // Act
            int productCount = _homePage!.GetProductCount();

            // Assert
            Assert.That(productCount, Is.GreaterThan(0), "Inventory should display products");
            Log.Information($"Inventory displays {productCount} products");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify product details page opens on product click")]
        public void ClickProductAndViewDetails()
        {
            // Act
            var productDetailsPage = _homePage!.ClickFirstProduct();

            // Assert
            Assert.That(productDetailsPage.IsProductDetailsPageLoaded(), Is.True, "Product details page should load");
            Assert.That(productDetailsPage.GetProductTitle(), Is.Not.Empty, "Product title should not be empty");
            Log.Information("Product details page loaded successfully");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify multiple products can be clicked")]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void ClickMultipleProductsByIndex(int productIndex)
        {
            // Act
            var productDetailsPage = _homePage!.ClickProductByIndex(productIndex);

            // Assert
            Assert.That(productDetailsPage.IsProductDetailsPageLoaded(), Is.True, "Product details page should load");
            string title = productDetailsPage.GetProductTitle();
            Assert.That(title, Is.Not.Empty, "Product title should not be empty");
            Log.Information($"Product {productIndex} details: {title}");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify product information is complete")]
        public void VerifyProductInformationComplete()
        {
            // Act
            var productDetailsPage = _homePage!.ClickFirstProduct();
            string title = productDetailsPage.GetProductTitle();
            string price = productDetailsPage.GetProductPrice();
            string description = productDetailsPage.GetProductDescription();

            // Assert
            Assert.That(title, Is.Not.Empty, "Product title should not be empty");
            Assert.That(price, Is.Not.Empty, "Product price should not be empty");
            Assert.That(description, Is.Not.Empty, "Product description should not be empty");
            Log.Information($"Product info complete - Title: {title}, Price: {price}");
        }

        [Test]
        [Category("Functional")]
        [Description("Add product to cart from details page")]
        public void AddProductToCartFromDetailsPage()
        {
            // Arrange
            var productDetailsPage = _homePage!.ClickFirstProduct();
            string productTitle = productDetailsPage.GetProductTitle();

            // Act
            productDetailsPage.AddToCart();

            // Assert
            Log.Information($"Product '{productTitle}' added to cart successfully");
        }

        [Test]
        [Category("Functional")]
        [Description("Return from product details to inventory")]
        public void ReturnToInventoryFromProductDetails()
        {
            // Arrange
            var productDetailsPage = _homePage!.ClickFirstProduct();

            // Act
            var inventoryPage = productDetailsPage.GoBackToProducts();

            // Assert
            Assert.That(inventoryPage.IsHomePageLoaded(), Is.True, "Should return to inventory page");
            Assert.That(inventoryPage.GetProductCount(), Is.GreaterThan(0), "Should display products");
            Log.Information("Returned to inventory page successfully");
        }

        [Test]
        [Category("Functional")]
        [Description("Browse through multiple products")]
        public void BrowseMultipleProducts()
        {
            // Act & Assert - Click through multiple products
            for (int i = 0; i < 3; i++)
            {
                var productDetailsPage = _homePage!.ClickProductByIndex(i);
                Assert.That(productDetailsPage.IsProductDetailsPageLoaded(), Is.True, $"Product {i} details should load");
                
                // Return to inventory
                _homePage = productDetailsPage.GoBackToProducts();
                Assert.That(_homePage.IsHomePageLoaded(), Is.True, $"Should return to inventory from product {i}");
            }
            
            Log.Information("Successfully browsed through multiple products");
        }
    }
}
