using NUnit.Framework;
using Serilog;
using TakealotAutomation.Pages;

namespace TakealotAutomation.Tests
{
    /// <summary>
    /// Test suite for authentication functionality
    /// </summary>
    [TestFixture]
    public class AuthenticationTests : BaseTest
    {
        private HomePage? _homePage;
        private const string ValidEmail = "testuser@example.com";
        private const string ValidPassword = "TestPassword123!";
        private const string InvalidEmail = "invalid@example.com";
        private const string InvalidPassword = "WrongPassword123!";

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _homePage = new HomePage(Driver!);
        }

        [Test]
        [Category("Authentication")]
        [Description("Verify login page loads")]
        public void VerifyLoginPageLoads()
        {
            // Arrange
            var accountPage = _homePage!.GoToAccount();

            // Act
            var loginPage = accountPage.GoToLogin();

            // Assert
            Assert.That(loginPage.IsLoginPageLoaded(), Is.True, "Login page should load");
            Log.Information("Login page loaded successfully");
        }

        [Test]
        [Category("Authentication")]
        [Description("Verify login with valid credentials")]
        public void LoginWithValidCredentials()
        {
            // Arrange
            var accountPage = _homePage!.GoToAccount();
            var loginPage = accountPage.GoToLogin();

            // Act
            var homePage = loginPage.Login(ValidEmail, ValidPassword);

            // Assert
            Assert.That(homePage.IsHomePageLoaded(), Is.True, "Should return to homepage after login");
            Log.Information("Login with valid credentials successful");
        }

        [Test]
        [Category("Authentication")]
        [Description("Verify login with invalid credentials displays error")]
        public void LoginWithInvalidCredentials()
        {
            // Arrange
            var accountPage = _homePage!.GoToAccount();
            var loginPage = accountPage.GoToLogin();

            // Act
            loginPage.Login(InvalidEmail, InvalidPassword);

            // Assert
            Assert.That(loginPage.IsErrorMessageDisplayed(), Is.True, "Error message should be displayed");
            Assert.That(loginPage.GetErrorMessage(), Is.Not.Empty, "Error message should not be empty");
            Log.Information("Invalid credentials error message displayed");
        }

        [Test]
        [Category("Authentication")]
        [Description("Verify signup page loads")]
        public void VerifySignUpPageLoads()
        {
            // Arrange
            var accountPage = _homePage!.GoToAccount();

            // Act
            var signUpPage = accountPage.GoToSignUp();

            // Assert
            Assert.That(signUpPage.IsSignUpPageLoaded(), Is.True, "Signup page should load");
            Log.Information("Signup page loaded successfully");
        }

        [Test]
        [Category("Authentication")]
        [Description("Verify signup form can be filled")]
        public void FillSignUpForm()
        {
            // Arrange
            var accountPage = _homePage!.GoToAccount();
            var signUpPage = accountPage.GoToSignUp();
            string email = $"newuser_{DateTime.Now.Ticks}@example.com";
            string firstName = "John";
            string lastName = "Doe";
            string password = "NewPassword123!";
            string phone = "0123456789";

            // Act
            signUpPage.FillSignUpForm(email, firstName, lastName, password, phone);
            signUpPage.AcceptTerms();

            // Assert
            Log.Information($"Signup form filled for {email}");
            // Note: Additional assertions would verify form field values
        }

        [Test]
        [Category("Authentication")]
        [Description("Verify logout functionality")]
        public void LogoutUser()
        {
            // Arrange - First login
            var accountPage = _homePage!.GoToAccount();
            var loginPage = accountPage.GoToLogin();
            loginPage.Login(ValidEmail, ValidPassword);

            // Re-navigate to account
            accountPage = _homePage!.GoToAccount();

            // Act
            var homePage = accountPage.Logout();

            // Assert
            Assert.That(homePage.IsHomePageLoaded(), Is.True, "Should return to homepage after logout");
            Log.Information("Logout successful");
        }
    }
}
