using OpenQA.Selenium;
using Serilog;

namespace TakealotAutomation.Pages
{
    /// <summary>
    /// Page object for user login
    /// </summary>
    public class LoginPage : Core.BasePage
    {
        // Locators
        private readonly By _emailInputLocator = By.XPath("//input[@type='email']");
        private readonly By _passwordInputLocator = By.XPath("//input[@type='password']");
        private readonly By _loginButtonLocator = By.XPath("//button[contains(text(), 'Login')]");
        private readonly By _forgotPasswordLinkLocator = By.XPath("//a[contains(text(), 'Forgot Password')]");
        private readonly By _signUpLinkLocator = By.XPath("//a[contains(text(), 'Sign Up')]");
        private readonly By _errorMessageLocator = By.XPath("//div[contains(@class, 'error-message')]");

        public LoginPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Logs in with email and password
        /// </summary>
        public HomePage Login(string email, string password)
        {
            Log.Information("Logging in");
            WaitAndSendKeys(_emailInputLocator, email);
            WaitAndSendKeys(_passwordInputLocator, password);
            WaitAndClick(_loginButtonLocator);
            return new HomePage(Driver);
        }

        /// <summary>
        /// Gets error message
        /// </summary>
        public string GetErrorMessage()
        {
            string errorMessage = GetElementText(_errorMessageLocator);
            Log.Information($"Error message: {errorMessage}");
            return errorMessage;
        }

        /// <summary>
        /// Checks if error message is displayed
        /// </summary>
        public bool IsErrorMessageDisplayed()
        {
            return IsElementPresent(_errorMessageLocator);
        }

        /// <summary>
        /// Navigates to forgot password
        /// </summary>
        public void GoToForgotPassword()
        {
            Log.Information("Going to forgot password");
            WaitAndClick(_forgotPasswordLinkLocator);
        }

        /// <summary>
        /// Navigates to sign up
        /// </summary>
        public SignUpPage GoToSignUp()
        {
            Log.Information("Going to sign up");
            WaitAndClick(_signUpLinkLocator);
            return new SignUpPage(Driver);
        }

        /// <summary>
        /// Verifies login page is loaded
        /// </summary>
        public bool IsLoginPageLoaded()
        {
            bool isLoaded = IsElementPresent(_emailInputLocator) && IsElementPresent(_passwordInputLocator);
            Log.Information($"Login page loaded: {isLoaded}");
            return isLoaded;
        }
    }
}
