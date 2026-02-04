using NUnit.Framework;
using Serilog;
using TakealotAutomation.Pages;

namespace TakealotAutomation.Tests
{
    /// <summary>
    /// Test suite for checkout and order completion
    /// </summary>
    [TestFixture]
    public class CheckoutTests : BaseTest
    {
        private HomePage? _homePage;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _homePage = new HomePage(Driver!);
        }

        [Test]
        [Category("Checkout")]
        [Description("Verify checkout page loads with valid product")]
        public void VerifyCheckoutPageLoads()
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
            Log.Information("Checkout page verified");
        }

        [Test]
        [Category("Checkout")]
        [Description("Fill customer information on checkout")]
        public void FillCustomerInformation()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");
            var productDetailsPage = searchResultsPage.ClickFirstProduct();
            productDetailsPage.AddToCart();
            var cartPage = _homePage.GoToCart();
            var checkoutPage = cartPage.ProceedToCheckout();

            // Act
            checkoutPage.FillCustomerInformation(
                "customer@example.com",
                "John",
                "Doe",
                "0123456789"
            );

            // Assert
            Log.Information("Customer information filled successfully");
        }

        [Test]
        [Category("Checkout")]
        [Description("Fill delivery address on checkout")]
        public void FillDeliveryAddress()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");
            var productDetailsPage = searchResultsPage.ClickFirstProduct();
            productDetailsPage.AddToCart();
            var cartPage = _homePage.GoToCart();
            var checkoutPage = cartPage.ProceedToCheckout();

            // Act
            checkoutPage.FillDeliveryAddress(
                "123 Main Street",
                "Johannesburg",
                "2000"
            );

            // Assert
            Log.Information("Delivery address filled successfully");
        }

        [Test]
        [Category("Checkout")]
        [Description("Complete checkout process")]
        public void CompleteCheckoutProcess()
        {
            // Arrange
            var searchResultsPage = _homePage!.SearchForProduct("phone");
            var productDetailsPage = searchResultsPage.ClickFirstProduct();
            productDetailsPage.AddToCart();
            var cartPage = _homePage.GoToCart();
            var checkoutPage = cartPage.ProceedToCheckout();

            // Act
            checkoutPage.FillCustomerInformation(
                "customer@example.com",
                "John",
                "Doe",
                "0123456789"
            );
            checkoutPage.FillDeliveryAddress(
                "123 Main Street",
                "Johannesburg",
                "2000"
            );
            checkoutPage.ContinueToPayment();

            // Assert
            Log.Information("Checkout process completed");
            // Note: Payment page would have additional assertions
        }
    }
}
