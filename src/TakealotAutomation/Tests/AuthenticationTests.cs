using System;
using NUnit.Framework;
using Serilog;
using TakealotAutomation.Pages;

namespace TakealotAutomation.Tests
{
    /// <summary>
    /// Test suite for Sauce Demo authentication functionality
    /// </summary>
    [TestFixture]
    public class AuthenticationTests : BaseTest
    {
        private LoginPage? _loginPage;
        // Sauce Demo test credentials
        private const string ValidUsername = "standard_user";
        private const string ValidPassword = "secret_sauce";
        private const string LockedOutUsername = "locked_out_user";
        private const string InvalidUsername = "invalid_user";
        private const string InvalidPassword = "wrong_password";

        [SetUp]
        public void SetUp()
        {
            _loginPage = new LoginPage(Driver!);
        }

        [Test]
        [Category("Authentication")]
        [Category("Smoke")]
        [Description("Verify login page loads")]
        public void VerifyLoginPageLoads()
        {
            // Assert
            Assert.That(_loginPage!.IsLoginPageLoaded(), Is.True, "Login page should load");
            Log.Information("Login page loaded successfully");
        }

        [Test]
        [Category("Authentication")]
        [Category("Smoke")]
        [Description("Verify login with valid credentials")]
        public void LoginWithValidCredentials()
        {
            // Act
            var homePage = _loginPage!.Login(ValidUsername, ValidPassword);

            // Assert
            Assert.That(homePage.IsHomePageLoaded(), Is.True, "Should return to inventory page after login");
            Assert.That(homePage.GetProductCount(), Is.GreaterThan(0), "Should display products");
            Log.Information("Login with valid credentials successful");
        }

        [Test]
        [Category("Authentication")]
        [Description("Verify login with invalid credentials displays error")]
        public void LoginWithInvalidCredentials()
        {
            // Act
            _loginPage!.Login(InvalidUsername, InvalidPassword);

            // Assert
            Assert.That(_loginPage.IsErrorMessageDisplayed(), Is.True, "Error message should be displayed");
            string errorMessage = _loginPage.GetErrorMessage();
            Assert.That(errorMessage, Is.Not.Empty, "Error message should not be empty");
            Log.Information($"Invalid credentials error displayed: {errorMessage}");
        }

        [Test]
        [Category("Authentication")]
        [Description("Verify locked out user cannot login")]
        public void LoginWithLockedOutUser()
        {
            // Act
            _loginPage!.Login(LockedOutUsername, ValidPassword);

            // Assert
            Assert.That(_loginPage.IsErrorMessageDisplayed(), Is.True, "Error message should be displayed for locked out user");
            string errorMessage = _loginPage.GetErrorMessage();
            Assert.That(errorMessage.ToLower(), Does.Contain("locked"), "Error should indicate user is locked out");
            Log.Information("Locked out user cannot login");
        }

        [Test]
        [Category("Authentication")]
        [Category("Smoke")]
        [Description("Verify logout functionality")]
        public void LogoutUser()
        {
            // Arrange - First login
            var homePage = _loginPage!.Login(ValidUsername, ValidPassword);
            Assert.That(homePage.IsHomePageLoaded(), Is.True, "Should be logged in");

            // Act
            var loginPage = homePage.Logout();

            // Assert
            Assert.That(loginPage.IsLoginPageLoaded(), Is.True, "Should return to login page after logout");
            Log.Information("Logout successful");
        }
    }
}
