# Sauce Demo Automation Framework - Test Execution Guide

## Running Tests Locally

### Prerequisites
- .NET 8.0 or higher installed
- Chrome browser installed
- Administrator access to the machine

### Basic Execution

#### Run All Tests
```bash
dotnet test
```

#### Run Tests with Detailed Output
```bash
dotnet test --verbosity detailed
```

#### Run Tests in Release Mode
```bash
dotnet test --configuration Release
```

## Test Filtering

### Run Specific Category
```bash
# Smoke tests only
dotnet test --filter "Category=Smoke"

# Functional tests only
dotnet test --filter "Category=Functional"

# Authentication tests only
dotnet test --filter "Category=Authentication"

# Checkout tests only
dotnet test --filter "Category=Checkout"
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~HomePageTests"
```

### Run Specific Test Method
```bash
dotnet test --filter "Name=SearchForValidProduct"
```

### Combined Filters
```bash
# Smoke AND Functional tests
dotnet test --filter "Category=Smoke | Category=Functional"

# Exclude certain tests
dotnet test --filter "FullyQualifiedName!~SlowTests"
```

## Test Reporting

### Generate NUnit Report
```bash
dotnet test --logger "nunit;LogFileName=test-report.xml"
```

### Generate Trx Report (Visual Studio format)
```bash
dotnet test --logger "trx;LogFileName=test-results.trx"
```

### Generate Console Report with Details
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Generate HTML Report
```bash
dotnet test --logger "html;LogFileName=test-report.html"
```

## Test Execution Strategies

### Quick Smoke Test (5 mins)
```bash
dotnet test --filter "Category=Smoke" --configuration Release
```

### Full Regression (30 mins)
```bash
dotnet test --configuration Release --logger "console;verbosity=normal"
```

### Critical Path Testing (15 mins)
```bash
dotnet test --filter "Category=Smoke | Category=Checkout" --configuration Release
```

## Configuration for Test Execution

Edit `appsettings.json` before running tests:

### Headless Mode (CI/CD)
```json
{
  "AppSettings": {
    "HeadlessMode": true
  }
}
```

### Local Development
```json
{
  "AppSettings": {
    "HeadlessMode": false
  }
}
```

### Extended Timeouts (Slow Networks)
```json
{
  "AppSettings": {
    "ImplicitWait": 15,
    "ExplicitWait": 20,
    "PageLoadTimeout": 45
  }
}
```

### Reduced Timeouts (Fast Networks)
```json
{
  "AppSettings": {
    "ImplicitWait": 5,
    "ExplicitWait": 10,
    "PageLoadTimeout": 20
  }
}
```

## Parallel Test Execution

### Run Tests in Parallel (4 threads)
```bash
dotnet test -- RunConfiguration.MaxCpuCount=4
```

### Single-threaded Execution (Sequential)
```bash
dotnet test -- RunConfiguration.MaxCpuCount=1
```

## Debugging Tests

### Run with Console Output
```bash
dotnet test --verbosity detailed
```

### Attach Debugger (Visual Studio)
```bash
# Open the project in Visual Studio
# Set breakpoints in the test code
# Test → Run Tests → Attach to Process
```

### Check Logs
```bash
# After test execution, check Logs directory
cd Logs
# Review the most recent log file
```

### Check Screenshots
```bash
# Failed tests capture screenshots
cd Screenshots
# Review the screenshot from the failed test
```

## Troubleshooting

### Tests Timeout
- Increase `ExplicitWait` in `appsettings.json`
- Check internet connectivity
- Verify Sauce Demo.com is accessible

### Tests Fail to Start Driver
```bash
# Ensure Chrome is installed
# Update WebDriver: dotnet restore
# Check Chrome path in environment
```

### Element Not Found Errors
- Verify website structure hasn't changed
- Update locators in page objects
- Increase wait times temporarily

### Flaky Tests
- Review test logs in Logs/ directory
- Check screenshots in Screenshots/ directory
- Increase wait times for critical operations

## CI/CD Integration

### GitHub Actions
Tests run automatically on:
- Push to main or develop branch
- Pull requests to main or develop

### Local Pre-commit Testing
```bash
#!/bin/bash
# Save as .git/hooks/pre-commit and chmod +x

dotnet build
if [ $? -ne 0 ]; then
    echo "Build failed. Commit aborted."
    exit 1
fi

dotnet test --filter "Category=Smoke"
if [ $? -ne 0 ]; then
    echo "Tests failed. Commit aborted."
    exit 1
fi
```

## Test Metrics

### Run Tests and Show Summary
```bash
dotnet test --verbosity normal
```

Expected output includes:
- Total tests run
- Passed count
- Failed count
- Skipped count
- Total execution time

## Performance Optimization

### Skip UI Tests (Unit Only)
```bash
# Only if you have unit tests
dotnet test --filter "Category!=UI"
```

### Run Only Essential Tests
```bash
dotnet test --filter "Category=Smoke"
```

### Cache WebDriver
- WebDriverManager caches drivers automatically
- First run downloads Chrome WebDriver
- Subsequent runs use cached version

## Post-Execution Review

1. **Check Test Results**
   ```bash
   cat test-results.trx  # View Trx report
   ```

2. **Review Logs**
   ```bash
   ls -la Logs/          # List log files
   tail -f Logs/log-{date}.txt
   ```

3. **Examine Screenshots** (if failures)
   ```bash
   ls Screenshots/       # View failed test screenshots
   ```

4. **Analyze Failures**
   - Check exception messages in logs
   - Review screenshots for visual clues
   - Verify test data is still valid

## Automated Test Run Example

```bash
#!/bin/bash
# run-tests.sh

echo "Building project..."
dotnet build --configuration Release

echo "Running smoke tests..."
dotnet test --configuration Release --filter "Category=Smoke" --logger "trx;LogFileName=smoke-results.trx"

echo "Running functional tests..."
dotnet test --configuration Release --filter "Category=Functional" --logger "trx;LogFileName=functional-results.trx"

echo "Copying artifacts..."
mkdir -p artifacts
cp test-results/*.trx artifacts/
cp -r Screenshots/ artifacts/
cp -r Logs/ artifacts/

echo "Test execution completed!"
```

Run with:
```bash
chmod +x run-tests.sh
./run-tests.sh
```

---

For more information, see [README.md](README.md)

