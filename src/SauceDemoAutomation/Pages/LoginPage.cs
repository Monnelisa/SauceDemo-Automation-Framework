using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;

namespace SauceDemoAutomation.Pages
{
    /// <summary>
    /// Page object for Sauce Demo login page
    /// </summary>
    public class LoginPage : Core.BasePage
    {
        // Locators for Sauce Demo
        private readonly By _usernameInputLocator = By.Id("user-name");
        private readonly By _passwordInputLocator = By.Id("password");
        private readonly By _loginButtonLocator = By.Id("login-button");
        private readonly By _errorMessageLocator = By.XPath("//h3[@data-test='error']");
        private readonly By _loginContainerLocator = By.ClassName("login_container");

        public LoginPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Logs in with username and password
        /// </summary>
        public HomePage Login(string username, string password)
        {
            Log.Information($"Logging in with username: {username}");
            WaitAndSendKeys(_usernameInputLocator, username);
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
        /// Verifies login page is loaded
        /// </summary>
        public bool IsLoginPageLoaded()
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
                bool isLoaded = wait.Until(driver =>
                {
                    var usernameInputs = driver.FindElements(_usernameInputLocator);
                    var passwordInputs = driver.FindElements(_passwordInputLocator);
                    var loginButtons = driver.FindElements(_loginButtonLocator);

                    return usernameInputs.Count > 0
                           && usernameInputs[0].Displayed
                           && passwordInputs.Count > 0
                           && passwordInputs[0].Displayed
                           && loginButtons.Count > 0
                           && loginButtons[0].Displayed;
                });

                Log.Information($"Login page loaded: {isLoaded}");
                return isLoaded;
            }
            catch (WebDriverTimeoutException ex)
            {
                Log.Warning($"Login page loaded: false - {ex.Message}");
                return false;
            }
        }
    }
}
