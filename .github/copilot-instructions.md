# AI Coding Agent Instructions - Takealot Automation Framework

This guide helps AI agents understand and extend the Takealot test automation framework efficiently.

## Project Overview

**Takealot-Automation-Framework** is an enterprise-grade C# Selenium WebDriver test automation framework for ecommerce regression testing on Takealot.com. It demonstrates professional QA practices suitable for portfolio and production use.

**Tech Stack:** C# 13, .NET 10.0, Selenium WebDriver 4.15, NUnit 4.1, Serilog 3.1

## Architecture & Key Components

### Layered Structure

```
Core/
  ├─ DriverFactory.cs      → Single source for WebDriver initialization with implicit waits
  ├─ BasePage.cs           → Base class for all page objects with wait/interaction methods
  └─ Defines wait strategies: implicit (10s default), explicit (15s), custom retries

Configuration/
  └─ ConfigurationManager.cs → Centralizes appsettings.json access (immutable singleton pattern)
     Settings: BaseUrl, BrowserType, Timeouts, HeadlessMode, Paths

Pages/ (9 Page Objects)
  └─ All inherit from BasePage
     Pattern: Private readonly By locators → Public action methods → Return next page object
     Examples: HomePage.SearchForProduct() returns SearchResultsPage

Tests/ (5 Test Suites)
  ├─ BaseTest.cs           → NUnit [TestFixture] with lifecycle: OneTimeSetUp → SetUp/TearDown → OneTimeTearDown
  ├─ AuthenticationTests, HomePageTests, CartFunctionalityTests, etc.
  └─ Test categories: [Category("Smoke")], [Category("Functional")], [Category("Authentication")], [Category("Checkout")]

Utilities/
  ├─ WaitHelper.cs         → Custom retry logic (3 attempts, 1s delay by default)
  ├─ DataHelper.cs         → Test data generation
  └─ FileHelper.cs         → File operations
```

### Data Flow

```
Test (Arrange) → PageObject.Action() → Selenium Wait (BasePage) → WebDriver → Browser
              ← Assert Result   ← Return PageObject/Value
```

## Critical Developer Workflows

### Running Tests

```bash
# All tests with implicit filtering
dotnet test

# Specific category (most common)
dotnet test --filter "Category=Smoke"
dotnet test --filter "Category=Functional"

# Specific test class
dotnet test --filter "FullyQualifiedName~CartFunctionalityTests"

# With reports
dotnet test --logger "nunit;LogFileName=test-report.xml"
```

**Key Insight:** Test filtering by Category is the primary workflow—category attributes drive CI/CD pipelines.

### Debug Workflow

1. **Failures auto-capture:** BaseTest.TearDown() saves screenshot on failure to `Screenshots/` 
2. **Logging auto-functions:** Serilog logs all interactions to `Logs/log-YYYYMMDD.txt` (daily rolling)
3. **Headless mode toggle:** Set `HeadlessMode: false` in appsettings.json for visual debugging

## Code Patterns & Conventions

### Page Object Model (POM)

```csharp
public class ProductDetailsPage : BasePage
{
    // 1. Private readonly locators (single source of truth for selectors)
    private readonly By _addToCartButton = By.XPath("//button[@data-test='add-to-cart']");
    private readonly By _productTitle = By.ClassName("product-title");
    
    public ProductDetailsPage(IWebDriver driver) : base(driver) { }
    
    // 2. Public action methods (user-facing interactions)
    public CartPage AddToCart()
    {
        WaitAndClick(_addToCartButton);  // Built-in explicit wait from BasePage
        return new CartPage(Driver);      // Return next page for fluent test chains
    }
    
    // 3. Verification methods (for assertions)
    public bool IsProductLoaded() => IsElementPresent(_productTitle);
}
```

**Enforcement:** All page classes must inherit BasePage, use WaitAndClick/WaitForElementToBeVisible from base, never use Thread.Sleep()

### Wait Strategy (3-Level)

```csharp
// Level 1: Implicit (set once in DriverFactory, applies to all FindElement)
driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

// Level 2: Explicit (for critical operations—used in BasePage methods)
protected IWebElement WaitForElementToBeVisible(By locator)
{
    return Wait.Until(ExpectedConditions.ElementIsVisible(locator));
}

// Level 3: Custom Retry (WaitHelper for complex scenarios)
WaitHelper.RetryAction(() => CheckoutPage.CompletePayment(), maxAttempts: 3, delayMs: 1000);
```

**Key Rule:** Never increase timeouts arbitrarily—investigate flaky tests root cause instead.

### Test Structure (NUnit Template)

```csharp
[TestFixture]
public class SearchFunctionalityTests : BaseTest
{
    private HomePage _homePage;
    
    [SetUp]
    public new void SetUp()
    {
        base.SetUp();  // Initializes WebDriver + Logging
        _homePage = new HomePage(Driver);
    }
    
    [Test]
    [Category("Smoke")]
    [Description("User can search and find valid product")]
    public void SearchForValidProduct()
    {
        // Arrange: Set test data
        string searchTerm = "Samsung Galaxy S24";
        
        // Act: Perform action
        var results = _homePage.SearchForProduct(searchTerm);
        
        // Assert: Verify outcome
        Assert.That(results.IsSearchResultsDisplayed(), Is.True);
        Assert.That(results.GetProductCount(), Is.GreaterThan(0));
    }
}
```

**Naming Conventions:**
- Classes: `[FeatureName]Tests`
- Methods: `[Action]_[Scenario]_[ExpectedResult]`
- Test categories drive CI/CD (Smoke runs full suite; Functional runs selective tests)

### Configuration Pattern

```csharp
// ALWAYS use ConfigurationManager—never hardcode values
string baseUrl = ConfigurationManager.GetBaseUrl();
int explicitWait = ConfigurationManager.GetExplicitWait();
bool isHeadless = ConfigurationManager.IsHeadlessMode();

// appsettings.json is single source of truth
{
  "AppSettings": {
    "BaseUrl": "https://www.takealot.com",
    "BrowserType": "Chrome",
    "ImplicitWait": 10,
    "ExplicitWait": 15,
    "HeadlessMode": true
  }
}
```

## Integration Points & Dependencies

### Selenium WebDriver Lifecycle

```
DriverFactory.CreateDriver()  → Initialize Chrome with options (headless, notifications disabled)
                             → Set implicit, page load timeouts
                             → Navigate to BaseUrl
                             → Return IWebDriver instance

BaseTest.SetUp()            → Create WebDriver via DriverFactory
                             → Instantiate page objects via constructor

BaseTest.TearDown()         → Capture screenshot if test failed
                             → Call DriverFactory.QuitDriver()
                             → Flush Serilog
```

### External Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Selenium.WebDriver | 4.15.0 | Browser automation |
| NUnit | 4.1.0 | Test framework (assertions, lifecycle) |
| Serilog | 3.1.1 | Structured logging (File + Console) |
| WebDriverManager | 2.16.0 | Driver version management |
| Microsoft.Extensions.Configuration | 8.0.0 | Config file binding |

**Key Interaction:** WebDriverManager auto-handles ChromeDriver versioning—never manually manage driver executables.

## Common Tasks & Patterns

### Adding New Page Object

1. **Create class in `Pages/` inheriting BasePage**
2. **Add private readonly By locators**
3. **Implement action methods returning page objects**
4. **Add verification method(s) for assertions**

### Adding New Test Suite

1. **Create class in `Tests/` inheriting BaseTest**
2. **Add [SetUp] initializing page objects**
3. **Use [Category("Functional")] or other category**
4. **Follow Arrange-Act-Assert structure**

### Debugging Flaky Tests

1. **Check logs first:** `Logs/log-*.txt` for element timing issues
2. **Increase explicit wait only if systemic** (default 15s is generous)
3. **Verify locators are stable** (avoid index-based, prefer data attributes)
4. **Use WaitHelper.RetryAction() for transient failures** rather than increasing timeouts

### Modifying Configuration

1. **Update `appsettings.json` with new setting**
2. **Add getter method to `ConfigurationManager.cs`**
3. **Access only via ConfigurationManager—never direct config reads**

## Anti-Patterns to Avoid

❌ `Thread.Sleep()` — Use WaitForElementToBeVisible or WaitHelper instead  
❌ Hardcoded wait times — Use ConfigurationManager getters  
❌ Direct By locator usage in tests — Create page objects  
❌ Modifying Driver directly in tests — Use page object methods  
❌ Multiple page instantiations per test — Store in [SetUp] fields  
❌ Ignoring test categories — All new tests must have [Category(...)]  
❌ Screenshots on success — Captured only on failure (BaseTest.TearDown)  

## Project Statistics

- **25+ files:** Configuration, Core, Pages (9), Tests (5), Utilities (3), Docs (7)
- **23 test cases:** Across 5 test suites
- **9 page objects:** Covering Takealot user journeys (Search, Cart, Checkout, Auth)
- **Fully documented:** README, BEST-PRACTICES, TEST-EXECUTION guides

## Quick Reference Files

| File | Purpose |
|------|---------|
| [Core/BasePage.cs](../Core/BasePage.cs) | Base class pattern—review for wait strategy implementation |
| [Core/DriverFactory.cs](../Core/DriverFactory.cs) | WebDriver initialization—copy for multi-browser support |
| [Tests/BaseTest.cs](../Tests/BaseTest.cs) | Test lifecycle—copy for new test suites |
| [Configuration/ConfigurationManager.cs](../Configuration/ConfigurationManager.cs) | Config access—add getters for new settings |
| [appsettings.json](../appsettings.json) | Runtime configuration—update for environment changes |
| [Pages/HomePage.cs](../Pages/HomePage.cs) | POM example—reference for new page objects |
| [Tests/SearchFunctionalityTests.cs](../Tests/SearchFunctionalityTests.cs) | Test template—copy structure for new test classes |

## When in Doubt

1. **Does code follow POM?** — All UI interactions must go through page objects
2. **Is it logged?** — Log critical actions for debugging; Serilog is pre-configured
3. **Is it configurable?** — If value changes by environment, add to appsettings.json
4. **Does test have category?** — Required for pipeline filtering
5. **Did I use explicit waits?** — No Thread.Sleep(); use BasePage methods
