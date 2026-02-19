# Test the API with these Swagger steps!

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setting up SA Funeral Supplies API" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET is installed
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ .NET SDK $dotnetVersion found" -ForegroundColor Green
} else {
    Write-Host "✗ .NET SDK not found!" -ForegroundColor Red
    Write-Host "  Install from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# Check SQL Server connectivity
Write-Host "Checking SQL Server connection..." -ForegroundColor Yellow
$sqlServices = Get-Service -Name "MSSQL*" -ErrorAction SilentlyContinue
if ($sqlServices) {
    $runningServices = $sqlServices | Where-Object {$_.Status -eq "Running"}
    if ($runningServices) {
        Write-Host "✓ SQL Server is running" -ForegroundColor Green
    } else {
        Write-Host "⚠ SQL Server service found but not running" -ForegroundColor Yellow
        Write-Host "  Start SQL Server to continue" -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠ SQL Server not found - make sure it's installed" -ForegroundColor Yellow
}

Write-Host ""

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Packages restored" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to restore packages" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Check if migrations exist
Write-Host "Checking database migrations..." -ForegroundColor Yellow
$migrationsFolder = "Migrations"
if (Test-Path $migrationsFolder) {
    Write-Host "✓ Migrations folder found" -ForegroundColor Green
} else {
    Write-Host "⚠ No migrations found - creating initial migration..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Initial migration created" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed to create migration" -ForegroundColor Red
        Write-Host "  Install EF Core tools: dotnet tool install --global dotnet-ef" -ForegroundColor Yellow
    }
}

Write-Host ""

# Apply migrations
Write-Host "Applying database migrations..." -ForegroundColor Yellow
dotnet ef database update
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Database updated successfully" -ForegroundColor Green
} else {
    Write-Host "⚠ Database update had issues (this is normal if database already exists)" -ForegroundColor Yellow
}

Write-Host ""

# Build project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Build successful" -ForegroundColor Green
} else {
    Write-Host "✗ Build failed" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "✨ Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📚 Quick Start Guide:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Run the API:" -ForegroundColor White
Write-Host "   dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Open your browser to:" -ForegroundColor White
Write-Host "   http://localhost:5000" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Login with:" -ForegroundColor White
Write-Host "   Email: admin@safuneralsupplies.co.za" -ForegroundColor Gray
Write-Host "   Password: Admin123!" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Copy the token from response" -ForegroundColor White
Write-Host ""
Write-Host "5. Click 'Authorize' button in Swagger UI" -ForegroundColor White
Write-Host ""
Write-Host "6. Paste token and start testing!" -ForegroundColor White
Write-Host ""
Write-Host "📖 For detailed instructions, see: GETTING_STARTED.md" -ForegroundColor Cyan
Write-Host ""

$runNow = Read-Host "Would you like to run the API now? (y/n)"
if ($runNow -eq "y" -or $runNow -eq "Y") {
    Write-Host ""
    Write-Host "🚀 Starting API... (Press Ctrl+C to stop)" -ForegroundColor Green
    Write-Host ""
    Start-Sleep -Seconds 1
    dotnet run
}
