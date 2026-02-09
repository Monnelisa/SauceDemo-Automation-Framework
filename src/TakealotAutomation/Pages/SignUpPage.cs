using OpenQA.Selenium;
using Serilog;

namespace TakealotAutomation.Pages
{
    /// <summary>
    /// Page object for user sign up
    /// </summary>
    public class SignUpPage : Core.BasePage
    {
        // Locators
        private readonly By _emailInputLocator = By.XPath("//input[@type='email']");
        private readonly By _passwordInputLocator = By.XPath("//input[@name='password']");
        private readonly By _confirmPasswordInputLocator = By.XPath("//input[@name='confirmPassword']");
        private readonly By _firstNameInputLocator = By.XPath("//input[@placeholder='First Name']");
        private readonly By _lastNameInputLocator = By.XPath("//input[@placeholder='Last Name']");
        private readonly By _phoneInputLocator = By.XPath("//input[@type='tel']");
        private readonly By _termsCheckboxLocator = By.XPath("//input[@type='checkbox']");
        private readonly By _signUpButtonLocator = By.XPath("//button[contains(text(), 'Sign Up')]");
        private readonly By _loginLinkLocator = By.XPath("//a[contains(text(), 'Login')]");
        private readonly By _errorMessageLocator = By.XPath("//div[contains(@class, 'error-message')]");
        private readonly By _successMessageLocator = By.XPath("//div[contains(@class, 'success-message')]");

        public SignUpPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Fills in signup form
        /// </summary>
        public void FillSignUpForm(string email, string firstName, string lastName, string password, string phone)
        {
            Log.Information("Filling signup form");
            WaitAndSendKeys(_emailInputLocator, email);
            WaitAndSendKeys(_firstNameInputLocator, firstName);
            WaitAndSendKeys(_lastNameInputLocator, lastName);
            WaitAndSendKeys(_passwordInputLocator, password);
            WaitAndSendKeys(_confirmPasswordInputLocator, password);
            WaitAndSendKeys(_phoneInputLocator, phone);
        }

        /// <summary>
        /// Accepts terms and conditions
        /// </summary>
        public void AcceptTerms()
        {
            Log.Information("Accepting terms");
            WaitAndClick(_termsCheckboxLocator);
        }

        /// <summary>
        /// Submits signup form
        /// </summary>
        public HomePage SignUp()
        {
            Log.Information("Submitting signup form");
            WaitAndClick(_signUpButtonLocator);
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
        /// Navigates to login
        /// </summary>
        public LoginPage GoToLogin()
        {
            Log.Information("Going to login");
            WaitAndClick(_loginLinkLocator);
            return new LoginPage(Driver);
        }

        /// <summary>
        /// Verifies signup page is loaded
        /// </summary>
        public bool IsSignUpPageLoaded()
        {
            bool isLoaded = IsElementPresent(_emailInputLocator) && IsElementPresent(_signUpButtonLocator);
            Log.Information($"Signup page loaded: {isLoaded}");
            return isLoaded;
        }
    }
}
