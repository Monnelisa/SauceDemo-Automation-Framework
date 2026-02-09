using OpenQA.Selenium;
using Serilog;

namespace TakealotAutomation.Pages
{
    /// <summary>
    /// Page object for account management
    /// </summary>
    public class AccountPage : Core.BasePage
    {
        // Locators
        private readonly By _loginLinkLocator = By.XPath("//a[contains(text(), 'Login')]");
        private readonly By _signUpLinkLocator = By.XPath("//a[contains(text(), 'Sign Up')]");
        private readonly By _logoutButtonLocator = By.XPath("//button[contains(text(), 'Logout')]");
        private readonly By _myOrdersLinkLocator = By.XPath("//a[contains(text(), 'My Orders')]");
        private readonly By _myWishlistLinkLocator = By.XPath("//a[contains(text(), 'My Wishlist')]");
        private readonly By _accountSettingsLinkLocator = By.XPath("//a[contains(text(), 'Account Settings')]");
        private readonly By _usernameLabelLocator = By.XPath("//span[contains(@class, 'username')]");

        public AccountPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Navigates to login page
        /// </summary>
        public LoginPage GoToLogin()
        {
            Log.Information("Navigating to login");
            WaitAndClick(_loginLinkLocator);
            return new LoginPage(Driver);
        }

        /// <summary>
        /// Navigates to signup page
        /// </summary>
        public SignUpPage GoToSignUp()
        {
            Log.Information("Navigating to sign up");
            WaitAndClick(_signUpLinkLocator);
            return new SignUpPage(Driver);
        }

        /// <summary>
        /// Logs out user
        /// </summary>
        public HomePage Logout()
        {
            Log.Information("Logging out");
            WaitAndClick(_logoutButtonLocator);
            return new HomePage(Driver);
        }

        /// <summary>
        /// Navigates to orders
        /// </summary>
        public void GoToMyOrders()
        {
            Log.Information("Navigating to my orders");
            WaitAndClick(_myOrdersLinkLocator);
        }

        /// <summary>
        /// Navigates to wishlist
        /// </summary>
        public void GoToMyWishlist()
        {
            Log.Information("Navigating to my wishlist");
            WaitAndClick(_myWishlistLinkLocator);
        }

        /// <summary>
        /// Gets username
        /// </summary>
        public string GetUsername()
        {
            string username = GetElementText(_usernameLabelLocator);
            Log.Information($"Username: {username}");
            return username;
        }

        /// <summary>
        /// Checks if user is logged in
        /// </summary>
        public bool IsUserLoggedIn()
        {
            bool isLoggedIn = IsElementPresent(_logoutButtonLocator);
            Log.Information($"User logged in: {isLoggedIn}");
            return isLoggedIn;
        }
    }
}
