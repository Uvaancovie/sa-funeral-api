#!/bin/bash

# Setup Script for SA Funeral Supplies .NET API

echo "========================================"
echo "SA Funeral Supplies .NET API Setup"
echo "========================================"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Check .NET SDK
echo -e "${YELLOW}Checking for .NET SDK...${NC}"
if command -v dotnet &> /dev/null; then
    dotnetVersion=$(dotnet --version)
    echo -e "${GREEN}✓ .NET SDK $dotnetVersion found${NC}"
else
    echo -e "${RED}✗ .NET SDK not found!${NC}"
    echo -e "${YELLOW}  Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0${NC}"
    exit 1
fi

echo ""

# Check if .env exists
echo -e "${YELLOW}Checking environment configuration...${NC}"
if [ -f ".env" ]; then
    echo -e "${GREEN}✓ .env file exists${NC}"
    
    # Check if values are configured
    if grep -q "your-username\|your-password\|change-this" .env; then
        echo -e "${YELLOW}⚠ Warning: .env file contains placeholder values${NC}"
        echo -e "${YELLOW}  Please update .env with your actual MongoDB connection string and JWT secret${NC}"
        echo ""
        read -p "Continue anyway? (y/n) " -n 1 -r
        echo
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            echo -e "${RED}Setup cancelled. Please configure .env file first.${NC}"
            exit 1
        fi
    else
        echo -e "${GREEN}✓ .env file looks configured${NC}"
    fi
else
    echo -e "${YELLOW}⚠ .env file not found${NC}"
    if [ -f ".env.example" ]; then
        echo -e "${YELLOW}  Creating .env from .env.example...${NC}"
        cp .env.example .env
        echo -e "${GREEN}✓ .env file created${NC}"
        echo -e "${YELLOW}⚠ Please edit .env file with your actual configuration before running the API${NC}"
        echo ""
        read -p "Continue with setup anyway? (y/n) " -n 1 -r
        echo
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            echo -e "${RED}Setup cancelled. Please configure .env file first.${NC}"
            exit 1
        fi
    else
        echo -e "${RED}✗ .env.example not found!${NC}"
        exit 1
    fi
fi

echo ""

# Restore dependencies
echo -e "${YELLOW}Restoring NuGet packages...${NC}"
if dotnet restore; then
    echo -e "${GREEN}✓ Packages restored successfully${NC}"
else
    echo -e "${RED}✗ Failed to restore packages${NC}"
    exit 1
fi

echo ""

# Build project
echo -e "${YELLOW}Building project...${NC}"
if dotnet build; then
    echo -e "${GREEN}✓ Build succeeded${NC}"
else
    echo -e "${RED}✗ Build failed${NC}"
    exit 1
fi

echo ""
echo -e "${CYAN}========================================${NC}"
echo -e "${GREEN}Setup Complete!${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""
echo -e "${CYAN}Next steps:${NC}"
echo "1. Ensure .env file has your MongoDB connection string and JWT secret"
echo "2. Run the API: dotnet run"
echo "3. Open browser to: https://localhost:5001"
echo "4. Login with: admin@safuneralsupplies.co.za / Admin123!"
echo ""
echo -e "${CYAN}For more information, see:${NC}"
echo "- README.md - Full documentation"
echo "- QUICKSTART.md - Quick start guide"
echo "- MIGRATION_GUIDE.md - Migration from Vercel"
echo ""

read -p "Would you like to run the API now? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo ""
    echo -e "${GREEN}Starting API...${NC}"
    echo -e "${YELLOW}Press Ctrl+C to stop${NC}"
    echo ""
    dotnet run
fi
