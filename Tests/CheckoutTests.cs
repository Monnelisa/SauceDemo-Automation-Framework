using NUnit.Framework;
using Serilog;
using TakealotAutomation.Pages;

namespace TakealotAutomation.Tests
{
    /// <summary>
    /// Test suite for Sauce Demo checkout and order completion
    /// </summary>
    [TestFixture]
    public class CheckoutTests : BaseTest
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
        [Category("Checkout")]
        [Category("Smoke")]
        [Description("Verify checkout page loads with valid product")]
        public void VerifyCheckoutPageLoads()
        {
            // Arrange
            _homePage!.AddFirstProductToCart();
            var cartPage = _homePage.GoToCart();

            // Act
            var checkoutPage = cartPage.ProceedToCheckout();

            // Assert
            Assert.That(checkoutPage.IsCheckoutPageLoaded(), Is.True, "Checkout page should load");
            Log.Information("Checkout page verified");
        }

        [Test]
        [Category("Checkout")]
        [Description("Fill checkout information")]
        public void FillCheckoutInformation()
        {
            // Arrange
            _homePage!.AddFirstProductToCart();
            var cartPage = _homePage.GoToCart();
            var checkoutPage = cartPage.ProceedToCheckout();

            // Act
            checkoutPage.FillCheckoutInformation("John", "Doe", "12345");

            // Assert
            Log.Information("Checkout information filled successfully");
        }

        [Test]
        [Category("Checkout")]
        [Category("Smoke")]
        [Description("Complete full checkout process and verify order")]
        public void CompleteCheckoutProcessSuccessfully()
        {
            // Arrange
            _homePage!.AddFirstProductToCart();
            var cartPage = _homePage.GoToCart();
            var checkoutPage = cartPage.ProceedToCheckout();

            // Act - Fill checkout info
            checkoutPage.FillCheckoutInformation("John", "Doe", "12345");
            
            // Continue to overview
            checkoutPage.ContinueToOverview();

            // Finish the order
            var confirmationPage = checkoutPage.FinishOrder();

            // Assert
            Assert.That(confirmationPage.IsOrderConfirmationDisplayed(), Is.True, "Order confirmation should be displayed");
            string confirmationMessage = confirmationPage.GetConfirmationMessage();
            Assert.That(confirmationMessage, Is.Not.Empty, "Confirmation message should not be empty");
            Log.Information($"Order completed successfully: {confirmationMessage}");
        }

        [Test]
        [Category("Checkout")]
        [Description("Cancel checkout and return to cart")]
        public void CancelCheckout()
        {
            // Arrange
            _homePage!.AddFirstProductToCart();
            var cartPage = _homePage.GoToCart();
            var checkoutPage = cartPage.ProceedToCheckout();

            // Act
            var returnedCartPage = checkoutPage.CancelCheckout();

            // Assert
            Assert.That(returnedCartPage.IsCartPageLoaded(), Is.True, "Should return to cart page");
            Assert.That(returnedCartPage.GetCartItemCount(), Is.GreaterThan(0), "Cart should still have items");
            Log.Information("Checkout cancelled successfully");
        }

        [Test]
        [Category("Checkout")]
        [Description("Checkout with missing first name displays error")]
        public void CheckoutWithMissingFirstName()
        {
            // Arrange
            _homePage!.AddFirstProductToCart();
            var cartPage = _homePage.GoToCart();
            var checkoutPage = cartPage.ProceedToCheckout();

            // Act - Fill info without first name
            checkoutPage.FillCheckoutInformation("", "Doe", "12345");
            checkoutPage.ContinueToOverview();

            // Assert
            string errorMessage = checkoutPage.GetErrorMessage();
            Assert.That(errorMessage, Is.Not.Empty, "Error message should be displayed for missing first name");
            Assert.That(errorMessage.ToLower(), Does.Contain("first"), "Error should mention first name");
            Log.Information($"Error message for missing first name: {errorMessage}");
        }

        [Test]
        [Category("Checkout")]
        [Description("After order confirmation, can return to shopping")]
        public void ReturnToShoppingAfterConfirmation()
        {
            // Arrange & Act - Complete checkout
            _homePage!.AddFirstProductToCart();
            var cartPage = _homePage.GoToCart();
            var checkoutPage = cartPage.ProceedToCheckout();
            checkoutPage.FillCheckoutInformation("Jane", "Smith", "54321");
            checkoutPage.ContinueToOverview();
            var confirmationPage = checkoutPage.FinishOrder();

            // Act - Return to shopping
            var homePage = confirmationPage.BackToHome();

            // Assert
            Assert.That(homePage.IsHomePageLoaded(), Is.True, "Should return to inventory page");
            Assert.That(homePage.GetProductCount(), Is.GreaterThan(0), "Should display products");
            Log.Information("Returned to shopping from confirmation page");
        }
    }
}
