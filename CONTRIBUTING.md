# Takealot Automation Framework - Contributing Guide

Thank you for considering contributing to the Takealot Automation Framework! This document provides guidelines and instructions for contributing.

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Focus on the code, not the person
- Help maintain a welcoming environment

## Getting Started

1. **Fork the repository**
   ```bash
   git clone https://github.com/yourusername/TakealotAutomation.git
   ```

2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Install dependencies**
   ```bash
   dotnet restore
   ```

4. **Build the project**
   ```bash
   dotnet build
   ```

## Development Guidelines

### Code Style

- Follow C# naming conventions
- Use meaningful variable and method names
- Add XML documentation comments for public methods
- Keep methods focused and single-responsibility

### Test Writing

1. **Use descriptive test names**
   ```csharp
   public void SearchForValidProduct_WithValidSearchTerm_ReturnsResults()
   ```

2. **Follow Arrange-Act-Assert pattern**
   ```csharp
   [Test]
   public void TestExample()
   {
       // Arrange
       var page = new HomePage(Driver);
       
       // Act
       var result = page.PerformAction();
       
       // Assert
       Assert.That(result, Is.True);
   }
   ```

3. **Add proper categories**
   ```csharp
   [Test]
   [Category("Smoke")]
   [Description("Clear description of what is tested")]
   public void TestMethod()
   ```

4. **Use meaningful assertions**
   ```csharp
   Assert.That(count, Is.GreaterThan(0), "Should return at least one product");
   ```

### Locator Management

- Keep locators private and readonly
- Use meaningful names (e.g., `_submitButtonLocator`)
- Prefer XPath when necessary, but use CSS selectors when possible for performance
- Add comments for complex locators

```csharp
private readonly By _complexElementLocator = By.XPath(
    "//div[@class='container']//button[contains(text(), 'Submit')]"
);
```

### Logging

- Use Serilog for all logging
- Log important actions and state changes
- Use appropriate log levels (Information, Warning, Error)

```csharp
Log.Information($"Searched for product: {searchTerm}");
Log.Error($"Error clicking element: {locator}");
```

## Commit Guidelines

- Use clear, descriptive commit messages
- Reference issue numbers when applicable
- Follow the format: `[Type] Description`

Examples:
- `[Feature] Add product filtering tests`
- `[Fix] Correct wait timeout in checkout page`
- `[Docs] Update README with new configuration options`
- `[Refactor] Extract common locators to base page`

## Pull Request Process

1. **Ensure code quality**
   - Run all tests locally: `dotnet test`
   - Build the project: `dotnet build`
   - Check for code style issues

2. **Create descriptive PR**
   - Clear title describing the change
   - Detailed description of what changed and why
   - Reference any related issues
   - Include before/after screenshots if UI-related

3. **Address feedback**
   - Respond to all review comments
   - Make requested changes
   - Re-request review when ready

## Testing Expectations

- Add tests for new functionality
- Ensure all tests pass locally
- Tests should be independent and not rely on execution order
- Use test data that is realistic but safe (no real personal data)

## Documentation

- Update README if adding new features
- Add XML documentation to public methods
- Include examples for complex functionality
- Update this guide if you change contribution process

## Reporting Issues

When reporting bugs:
- Provide detailed description
- Include steps to reproduce
- Add expected vs actual behavior
- Include relevant logs or screenshots
- Specify OS, browser, and .NET version

## Feature Requests

When requesting features:
- Describe the use case
- Explain why it's needed
- Provide implementation ideas if possible
- Discuss potential impact

## Questions?

- Check existing issues and discussions
- Review the README and project documentation
- Open a new discussion for general questions

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for making this project better!
