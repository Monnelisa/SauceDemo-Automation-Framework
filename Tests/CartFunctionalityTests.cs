using NUnit.Framework;
using Serilog;
using TakealotAutomation.Pages;

namespace TakealotAutomation.Tests
{
    /// <summary>
    /// Test suite for cart functionality
    /// </summary>
    [TestFixture]
    public class CartFunctionalityTests : BaseTest
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
        [Category("Functional")]
        [Description("Add product to cart and verify item count")]
        public void AddProductToCartAndVerify()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");
            var productDetailsPage = searchResultsPage.ClickFirstProduct();

            // Act
            productDetailsPage.AddToCart();
            var cartPage = _homePage.GoToCart();

            // Assert
            Assert.That(cartPage.GetCartItemCount(), Is.GreaterThan(0), "Cart should contain items");
            Assert.That(cartPage.GetCartTotal(), Is.Not.Empty, "Cart total should not be empty");
            Log.Information($"Cart contains {cartPage.GetCartItemCount()} items");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify cart total calculation")]
        public void VerifyCartTotal()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");
            var productDetailsPage = searchResultsPage.ClickFirstProduct();
            productDetailsPage.AddToCart();

            // Act
            var cartPage = _homePage.GoToCart();
            string cartTotal = cartPage.GetCartTotal();

            // Assert
            Assert.That(cartTotal, Is.Not.Empty, "Cart total should be calculated");
            Assert.That(cartTotal, Does.Contain("R"), "Cart total should contain currency symbol");
            Log.Information($"Cart total verified: {cartTotal}");
        }

        [Test]
        [Category("Functional")]
        [Description("Proceed to checkout from cart")]
        public void ProceedToCheckoutFromCart()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");
            var productDetailsPage = searchResultsPage.ClickFirstProduct();
            productDetailsPage.AddToCart();
            var cartPage = _homePage.GoToCart();

            // Act
            var checkoutPage = cartPage.ProceedToCheckout();

            // Assert
            Assert.That(checkoutPage.IsCheckoutPageLoaded(), Is.True, "Checkout page should load");
            Log.Information("Proceeded to checkout successfully");
        }

        [Test]
        [Category("Functional")]
        [Description("Apply promo code to cart")]
        public void ApplyPromoCodeToCart()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");
            var productDetailsPage = searchResultsPage.ClickFirstProduct();
            productDetailsPage.AddToCart();
            var cartPage = _homePage.GoToCart();
            string promoCode = "SAVE10";

            // Act
            cartPage.ApplyPromoCode(promoCode);

            // Assert
            Log.Information($"Promo code '{promoCode}' applied to cart");
            // Note: Additional assertion would check for discount applied
        }
    }
}
