using NUnit.Framework;
using Serilog;
using SauceDemoAutomation.Pages;

namespace SauceDemoAutomation.Tests
{
    /// <summary>
    /// Test suite for Sauce Demo cart functionality
    /// </summary>
    [TestFixture]
    public class CartFunctionalityTests : BaseTest
    {
        private LoginPage? _loginPage;
        private HomePage? _homePage;
        private const string ValidUsername = "standard_user";
        private const string ValidPassword = "secret_sauce";

        [SetUp]
        public void SetUp()
        {
            _loginPage = new LoginPage(Driver!);
            // Login before cart tests
            _homePage = _loginPage.Login(ValidUsername, ValidPassword);
        }

        [Test]
        [Category("Cart")]
        [Category("Smoke")]
        [Description("Verify empty cart displays correctly")]
        public void VerifyEmptyCartDisplay()
        {
            // Act
            var cartPage = _homePage!.GoToCart();

            // Assert
            Assert.That(cartPage.IsCartPageLoaded(), Is.True, "Cart page should load");
            Assert.That(cartPage.GetCartItemCount(), Is.EqualTo(0), "Empty cart should have 0 items");
            Log.Information("Empty cart verified");
        }

        [Test]
        [Category("Cart")]
        [Category("Smoke")]
        [Description("Add product to cart and verify item count")]
        public void AddProductToCartAndVerify()
        {
            // Arrange
            var productDetailsPage = _homePage!.ClickFirstProduct();

            // Act
            productDetailsPage.AddToCart();
            var cartPage = _homePage.GoToCart();

            // Assert
            Assert.That(cartPage.GetCartItemCount(), Is.GreaterThan(0), "Cart should contain items");
            Assert.That(cartPage.GetCartItemCount(), Is.EqualTo(1), "Cart should contain exactly 1 item");
            Log.Information($"Cart contains {cartPage.GetCartItemCount()} items");
        }

        [Test]
        [Category("Cart")]
        [Description("Add multiple products to cart")]
        public void AddMultipleProductsToCart()
        {
            // Arrange & Act - Add first product
            _homePage!.AddFirstProductToCart();
            
            // Go back to products and add another
            _homePage.GoToCart().ContinueShopping();
            _homePage.ClickProductByIndex(1).AddToCart();

            // Navigate to cart
            var cartPage = _homePage.GoToCart();

            // Assert
            Assert.That(cartPage.GetCartItemCount(), Is.EqualTo(2), "Cart should contain 2 items");
            Log.Information($"Successfully added multiple products to cart");
        }

        [Test]
        [Category("Cart")]
        [Description("Remove item from cart")]
        public void RemoveItemFromCart()
        {
            // Arrange - Add items to cart
            _homePage!.AddFirstProductToCart();
            var cartPage = _homePage.GoToCart();
            int initialCount = cartPage.GetCartItemCount();

            // Act
            if (initialCount > 0)
            {
                cartPage.RemoveItemAtIndex(0);
            }

            // Assert
            int finalCount = cartPage.GetCartItemCount();
            Assert.That(finalCount, Is.LessThan(initialCount), "Cart item count should decrease after removal");
            Log.Information($"Item removed from cart. Count: {initialCount} -> {finalCount}");
        }

        [Test]
        [Category("Cart")]
        [Category("Smoke")]
        [Description("Proceed to checkout from cart")]
        public void ProceedToCheckoutFromCart()
        {
            // Arrange
            _homePage!.AddFirstProductToCart();
            var cartPage = _homePage.GoToCart();

            // Act
            var checkoutPage = cartPage.ProceedToCheckout();

            // Assert
            Assert.That(checkoutPage.IsCheckoutPageLoaded(), Is.True, "Checkout page should load");
            Log.Information("Proceeded to checkout successfully");
        }

        [Test]
        [Category("Cart")]
        [Description("Continue shopping from cart returns to inventory")]
        public void ContinueShoppingFromCart()
        {
            // Arrange
            var cartPage = _homePage!.GoToCart();

            // Act
            var inventoryPage = cartPage.ContinueShopping();

            // Assert
            Assert.That(inventoryPage.IsHomePageLoaded(), Is.True, "Should return to inventory page");
            Assert.That(inventoryPage.GetProductCount(), Is.GreaterThan(0), "Should display products");
            Log.Information("Continued shopping - returned to inventory");
        }
    }
}
