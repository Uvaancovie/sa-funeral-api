# Setup Script for SA Funeral Supplies .NET API

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "SA Funeral Supplies .NET API Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check .NET SDK
Write-Host "Checking for .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ .NET SDK $dotnetVersion found" -ForegroundColor Green
} else {
    Write-Host "✗ .NET SDK not found!" -ForegroundColor Red
    Write-Host "  Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# Check if .env exists
Write-Host "Checking environment configuration..." -ForegroundColor Yellow
if (Test-Path ".env") {
    Write-Host "✓ .env file exists" -ForegroundColor Green
    
    # Check if values are configured
    $envContent = Get-Content ".env" -Raw
    if ($envContent -match "your-username" -or $envContent -match "your-password" -or $envContent -match "change-this") {
        Write-Host "⚠ Warning: .env file contains placeholder values" -ForegroundColor Yellow
        Write-Host "  Please update .env with your actual MongoDB connection string and JWT secret" -ForegroundColor Yellow
        Write-Host ""
        $continue = Read-Host "Continue anyway? (y/n)"
        if ($continue -ne "y" -and $continue -ne "Y") {
            Write-Host "Setup cancelled. Please configure .env file first." -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "✓ .env file looks configured" -ForegroundColor Green
    }
} else {
    Write-Host "⚠ .env file not found" -ForegroundColor Yellow
    if (Test-Path ".env.example") {
        Write-Host "  Creating .env from .env.example..." -ForegroundColor Yellow
        Copy-Item ".env.example" ".env"
        Write-Host "✓ .env file created" -ForegroundColor Green
        Write-Host "⚠ Please edit .env file with your actual configuration before running the API" -ForegroundColor Yellow
        Write-Host ""
        $continue = Read-Host "Continue with setup anyway? (y/n)"
        if ($continue -ne "y" -and $continue -ne "Y") {
            Write-Host "Setup cancelled. Please configure .env file first." -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "✗ .env.example not found!" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""

# Restore dependencies
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Packages restored successfully" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to restore packages" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Build project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Build succeeded" -ForegroundColor Green
} else {
    Write-Host "✗ Build failed" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Complete! " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Ensure .env file has your MongoDB connection string and JWT secret" -ForegroundColor White
Write-Host "2. Run the API: dotnet run" -ForegroundColor White
Write-Host "3. Open browser to: https://localhost:5001" -ForegroundColor White
Write-Host "4. Login with: admin@safuneralsupplies.co.za / Admin123!" -ForegroundColor White
Write-Host ""
Write-Host "For more information, see:" -ForegroundColor Cyan
Write-Host "- README.md - Full documentation" -ForegroundColor White
Write-Host "- QUICKSTART.md - Quick start guide" -ForegroundColor White
Write-Host "- MIGRATION_GUIDE.md - Migration from Vercel" -ForegroundColor White
Write-Host ""

$runNow = Read-Host "Would you like to run the API now? (y/n)"
if ($runNow -eq "y" -or $runNow -eq "Y") {
    Write-Host ""
    Write-Host "Starting API..." -ForegroundColor Green
    Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
    Write-Host ""
    dotnet run
}
