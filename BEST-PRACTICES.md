# Takealot Automation Framework - Best Practices Guide

## Overview

This guide documents the best practices implemented in the Takealot Automation Framework and provides guidance for maintaining and extending the project.

## 1. Page Object Model (POM) Pattern

### What & Why
- Encapsulates web page elements and interactions
- Improves test maintainability
- Reduces code duplication
- Makes tests more readable

### Implementation
```csharp
public class HomePage : BasePage
{
    // Private readonly locators
    private readonly By _searchInputLocator = By.XPath("...");
    private readonly By _searchButtonLocator = By.XPath("...");
    
    // Constructor
    public HomePage(IWebDriver driver) : base(driver) { }
    
    // Public methods for user actions
    public SearchResultsPage SearchForProduct(string productName)
    {
        WaitAndSendKeys(_searchInputLocator, productName);
        WaitAndClick(_searchButtonLocator);
        return new SearchResultsPage(Driver);
    }
    
    // Verification methods
    public bool IsHomePageLoaded() => IsElementPresent(_searchInputLocator);
}
```

### Best Practices
- Keep locators private and readonly
- Use meaningful names for locators
- Return page objects for navigation
- Include verification methods
- Document complex locators

## 2. Wait Strategies

### Three-Level Wait Approach
1. **Implicit Wait** - Applied globally to all FindElement calls
2. **Explicit Wait** - For specific critical operations
3. **Custom Waits** - Retry logic with configurable delays

### Implementation
```csharp
// Implicit wait (in DriverFactory)
driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

// Explicit wait (in BasePage)
protected IWebElement WaitForElementToBeVisible(By locator)
{
    return Wait.Until(ExpectedConditions.ElementIsVisible(locator));
}

// Custom retry (in WaitHelper)
WaitHelper.RetryAction(() => { /* action */ }, maxAttempts: 3, delayMs: 1000);
```

### Anti-Patterns to Avoid
❌ `Thread.Sleep()` for synchronization
❌ Hardcoded wait times
❌ Waiting longer than necessary
❌ Ignoring TimeoutException

## 3. Test Organization

### Test Structure
```csharp
[TestFixture]           // NUnit test class
public class ExampleTests : BaseTest
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
    [Description("Detailed description")]
    public void TestMethodName()
    {
        // Arrange - Set up test data and initial state
        string searchTerm = "phone";
        
        // Act - Perform the action being tested
        var results = _homePage.SearchForProduct(searchTerm);
        
        // Assert - Verify the expected outcome
        Assert.That(results.IsSearchResultsDisplayed(), Is.True);
    }
}
```

### Naming Conventions
- **Class Names:** `[FeatureName]Tests`
- **Method Names:** `[Action]_[Scenario]_[ExpectedResult]`
- **Variables:** camelCase for local, _camelCase for fields

### Test Categories
- `[Category("Smoke")]` - Quick validation tests
- `[Category("Functional")]` - Feature testing
- `[Category("Authentication")]` - Auth workflows
- `[Category("Checkout")]` - Payment processes

## 4. Configuration Management

### Externalized Settings
```json
{
  "AppSettings": {
    "BaseUrl": "https://www.takealot.com",
    "BrowserType": "Chrome",
    "ImplicitWait": 10,
    "ExplicitWait": 15
  }
}
```

### Accessing Configuration
```csharp
string baseUrl = ConfigurationManager.GetBaseUrl();
int wait = ConfigurationManager.GetExplicitWait();
bool headless = ConfigurationManager.IsHeadlessMode();
```

### Benefits
- No hardcoded values
- Environment-specific configs
- Runtime flexibility
- Easy to update

## 5. Logging & Debugging

### Serilog Implementation
```csharp
// In BasePage or test code
Log.Information($"Clicked element: {locator}");
Log.Warning($"Element took longer than expected: {locator}");
Log.Error($"Error clicking element: {ex.Message}");
```

### Log Levels
- **Information** - Normal flow, important actions
- **Warning** - Unexpected but recoverable issues
- **Error** - Exceptions and failures
- **Debug** - Detailed diagnostic information

### Output Locations
- **Console** - Real-time visibility during execution
- **File** - Daily rolling logs in `Logs/` directory
- **Screenshots** - Failed test screenshots in `Screenshots/`

## 6. Error Handling

### Good Error Handling
```csharp
try
{
    var element = WaitForElementToBeVisible(locator);
    element.Click();
    Log.Information($"Clicked element: {locator}");
}
catch (TimeoutException ex)
{
    Log.Error($"Timeout waiting for element: {locator}");
    throw new TimeoutException($"Element not found: {locator}", ex);
}
catch (Exception ex)
{
    Log.Error($"Unexpected error: {ex.Message}");
    TakeScreenshot("error");
    throw;
}
```

### Anti-Patterns
❌ Catching and ignoring exceptions
❌ Generic exception messages
❌ Not logging errors
❌ Not cleaning up resources

## 7. Data Generation

### Using DataHelper
```csharp
// Generate random email
string email = DataHelper.GenerateRandomEmail();

// Generate random string
string randomName = DataHelper.GenerateRandomString(10);

// Generate phone number
string phone = DataHelper.GenerateRandomPhoneNumber();

// Extract currency value
decimal price = DataHelper.ExtractCurrencyValue("R 1,999.99");
```

### Test Data Strategy
- Use realistic but safe data
- Avoid real personal information
- Generate unique data per test run
- Clean up after tests

## 8. CI/CD Integration

### GitHub Actions Workflow
- Triggered on push/PR
- Runs build and tests
- Generates reports
- Collects artifacts

### Local Pre-commit Checks
```bash
dotnet build                          # Build must succeed
dotnet test --filter "Category=Smoke" # Smoke tests must pass
```

## 9. Code Quality

### SOLID Principles

**Single Responsibility**
```csharp
// Good: Each class has one responsibility
BasePage - Handle waits and common interactions
HomePage - Handle homepage-specific operations
HomePageTests - Test homepage functionality

// Bad: Multiple responsibilities
public class HomePage { /* waits, api calls, database access */ }
```

**Open/Closed**
```csharp
// Good: Open for extension, closed for modification
public abstract class BasePage { /* common functionality */ }
public class HomePage : BasePage { /* specific functionality */ }

// Bad: Modifying base class for each new page
public class BasePage
{
    // Lots of if/else for different pages
}
```

**Liskov Substitution**
```csharp
// All pages should work the same way
var page = new HomePage(driver);   // or
var page = new SearchResultsPage(driver);
// Both work with BasePage methods
```

## 10. Test Data Management

### Parameterized Tests
```csharp
[TestCase("phone")]
[TestCase("headphones")]
[TestCase("tablet")]
public void SearchForValidProduct(string productName)
{
    // Test with multiple data values
}
```

### Test Data Organization
```csharp
// Constants for commonly used data
private const string ValidEmail = "test@example.com";
private const string ValidPassword = "Password123!";
private const string InvalidEmail = "invalid@example.com";

// Generated data for unique tests
string uniqueEmail = DataHelper.GenerateRandomEmail();
```

## 11. Performance Optimization

### Parallel Test Execution
```bash
dotnet test -- RunConfiguration.MaxCpuCount=4
```

### Optimize Waits
```bash
# Don't use maximum waits for all tests
# Use appropriate wait times based on operation

ImplicitWait: 5-10 seconds (normal)
ExplicitWait: 10-15 seconds (critical)
PageLoadTimeout: 20-30 seconds (page navigation)
```

### Headless Mode for CI/CD
```json
{
  "AppSettings": {
    "HeadlessMode": true
  }
}
```

## 12. Common Pitfalls & Solutions

### Pitfall 1: Hardcoded Waits
```csharp
// Bad
Thread.Sleep(5000);

// Good
WaitForElementToBeVisible(locator);
```

### Pitfall 2: No Error Logging
```csharp
// Bad
try { /* action */ } catch { }

// Good
try { /* action */ } 
catch (Exception ex) 
{ 
    Log.Error($"Error: {ex.Message}");
    throw;
}
```

### Pitfall 3: Tests Depending on Execution Order
```csharp
// Bad - Dependent on previous test
_cartId = GetCartFromPreviousTest();

// Good - Each test is independent
var cart = CreateNewCart();
```

### Pitfall 4: Flaky Locators
```csharp
// Bad
By.XPath("//button[1]")  // Brittle

// Good
By.XPath("//button[@id='submit-btn']")  // Specific and stable
```

## 13. Documentation Standards

### Method Documentation
```csharp
/// <summary>
/// Searches for a product on the homepage
/// </summary>
/// <param name="productName">The name of the product to search for</param>
/// <returns>SearchResultsPage object</returns>
public SearchResultsPage SearchForProduct(string productName)
{
    // Implementation
}
```

### Test Documentation
```csharp
[Test]
[Category("Functional")]
[Description("Verify that searching with a valid product name returns results")]
public void SearchForValidProduct()
{
    // Test implementation
}
```

## 14. Maintenance Guidelines

### Adding New Tests
1. Identify the page objects needed
2. Create page object if doesn't exist
3. Create test class with descriptive name
4. Add @Category and @Description
5. Document the test purpose
6. Run smoke tests to verify

### Updating Locators
1. Verify website changes
2. Update locator in page object
3. Run related tests
4. Update documentation if needed
5. Commit with clear message

### Adding New Page Objects
1. Create new class extending BasePage
2. Define all element locators
3. Create public action methods
4. Add verification methods
5. Include XML documentation
6. Update README

## 15. Testing Checklist

Before committing code:
- [ ] Code builds successfully
- [ ] All tests pass locally
- [ ] Smoke tests pass
- [ ] Code follows naming conventions
- [ ] Error handling is implemented
- [ ] Logging is added
- [ ] Documentation is updated
- [ ] No hardcoded values
- [ ] No Thread.Sleep() calls
- [ ] Comments are clear and helpful

---

## Summary

Following these best practices ensures:
✅ Maintainable, readable code
✅ Reliable, non-flaky tests
✅ Professional quality framework
✅ Easy to extend and modify
✅ Industry-standard implementations
✅ Production-ready codebase

---

**Last Updated:** February 2026  
**Version:** 1.0.0
