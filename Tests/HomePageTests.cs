using NUnit.Framework;
using Serilog;
using TakealotAutomation.Pages;

namespace TakealotAutomation.Tests
{
    /// <summary>
    /// Test suite for Sauce Demo inventory (homepage) functionality
    /// </summary>
    [TestFixture]
    public class HomePageTests : BaseTest
    {
        private LoginPage? _loginPage;
        private HomePage? _homePage;
        private const string ValidUsername = "standard_user";
        private const string ValidPassword = "secret_sauce";

        [SetUp]
        public void SetUp()
        {
            _loginPage = new LoginPage(Driver!);
            _homePage = _loginPage.Login(ValidUsername, ValidPassword);
        }

        [Test]
        [Category("Smoke")]
        [Description("Verify inventory page loads successfully")]
        public void VerifyInventoryPageLoads()
        {
            // Assert
            Assert.That(_homePage!.IsHomePageLoaded(), Is.True, "Inventory page should load successfully");
            Log.Information("Inventory page loaded successfully");
        }

        [Test]
        [Category("Smoke")]
        [Description("Verify page title is correct")]
        public void VerifyPageTitle()
        {
            // Act
            string pageTitle = _homePage!.GetPageTitle();

            // Assert
            Assert.That(pageTitle, Does.Contain("Swag Labs"), "Page title should contain 'Swag Labs'");
            Log.Information($"Page title verified: {pageTitle}");
        }

        [Test]
        [Category("Smoke")]
        [Description("Verify products are displayed on inventory")]
        public void VerifyProductsDisplayed()
        {
            // Act
            int productCount = _homePage!.GetProductCount();

            // Assert
            Assert.That(productCount, Is.GreaterThan(0), "Products should be displayed on inventory page");
            Log.Information($"Inventory page displays {productCount} products");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify clicking product opens product details")]
        public void ClickProductAndVerifyDetailsPage()
        {
            // Act
            var productDetailsPage = _homePage!.ClickFirstProduct();

            // Assert
            Assert.That(productDetailsPage.IsProductDetailsPageLoaded(), Is.True, "Product details page should load");
            Assert.That(productDetailsPage.GetProductTitle(), Is.Not.Empty, "Product title should not be empty");
            Assert.That(productDetailsPage.GetProductPrice(), Is.Not.Empty, "Product price should not be empty");
            Log.Information("Product details page verified");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify product details are displayed correctly")]
        public void VerifyProductDetailsDisplay()
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
            Log.Information($"Product details verified - Title: {title}, Price: {price}");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify cart icon is accessible from inventory")]
        public void VerifyCartAccessible()
        {
            // Act
            var cartPage = _homePage!.GoToCart();

            // Assert
            Assert.That(cartPage.IsCartPageLoaded(), Is.True, "Cart page should load");
            Log.Information("Cart accessed successfully from inventory");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify cart count updates")]
        public void VerifyCartCountUpdates()
        {
            // Arrange
            int initialCount = _homePage!.GetCartItemCount();

            // Act
            _homePage.AddFirstProductToCart();
            int updatedCount = _homePage.GetCartItemCount();

            // Assert
            Assert.That(updatedCount, Is.GreaterThan(initialCount), "Cart count should increase after adding item");
            Log.Information($"Cart count updated: {initialCount} -> {updatedCount}");
        }
    }
}
