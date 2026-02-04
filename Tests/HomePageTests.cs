using NUnit.Framework;
using Serilog;
using TakealotAutomation.Pages;

namespace TakealotAutomation.Tests
{
    /// <summary>
    /// Test suite for homepage functionality
    /// </summary>
    [TestFixture]
    public class HomePageTests : BaseTest
    {
        private HomePage? _homePage;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _homePage = new HomePage(Driver!);
        }

        [Test]
        [Category("Smoke")]
        [Description("Verify homepage loads successfully")]
        public void VerifyHomePageLoads()
        {
            // Assert
            Assert.That(_homePage!.IsHomePageLoaded(), Is.True, "Homepage should load successfully");
            Log.Information("Homepage loaded successfully");
        }

        [Test]
        [Category("Smoke")]
        [Description("Verify page title is correct")]
        public void VerifyPageTitle()
        {
            // Act
            string pageTitle = _homePage!.GetPageTitle();

            // Assert
            Assert.That(pageTitle, Does.Contain("Takealot"), "Page title should contain 'Takealot'");
            Log.Information($"Page title verified: {pageTitle}");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify search functionality")]
        public void VerifySearchFunctionality()
        {
            // Arrange
            string searchTerm = "laptop";

            // Act
            var searchResultsPage = _homePage!.SearchForProduct(searchTerm);

            // Assert
            Assert.That(searchResultsPage.IsSearchResultsPageLoaded(), Is.True, "Search results page should load");
            Assert.That(searchResultsPage.GetProductCount(), Is.GreaterThan(0), "Search should return results");
            Log.Information($"Search for '{searchTerm}' completed successfully");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify cart icon is accessible")]
        public void VerifyCartAccessible()
        {
            // Act
            var cartPage = _homePage!.GoToCart();

            // Assert
            Assert.That(cartPage.IsCartPageLoaded(), Is.True, "Cart page should load");
            Log.Information("Cart accessed successfully");
        }
    }
}
