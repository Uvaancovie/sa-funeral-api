# Quick Start Guide

## 1. Prerequisites Check

Ensure you have .NET 8.0 installed:
```bash
dotnet --version
```

If not installed, download from: https://dotnet.microsoft.com/download/dotnet/8.0

## 2. Configure Database

Edit the `.env` file with your MongoDB connection string:

```env
MONGODB_URI=mongodb+srv://your-username:your-password@your-cluster.mongodb.net/?retryWrites=true&w=majority
JWT_SECRET=a-random-secret-key-at-least-32-characters-long-change-this-in-production
```

## 3. Install Dependencies

```bash
cd SAFuneralSuppliesAPI
dotnet restore
```

## 4. Run the API

```bash
dotnet run
```

Or for development with auto-reload:
```bash
dotnet watch run
```

## 5. Test the API

Open your browser to: https://localhost:5001

You'll see the Swagger UI with all available endpoints.

## 6. Login to Test

Use the default admin credentials:
- **Email**: admin@safuneralsupplies.co.za
- **Password**: Admin123!

### Using Swagger UI:

1. Click on `POST /api/auth/login`
2. Click "Try it out"
3. Enter the credentials:
   ```json
   {
     "email": "admin@safuneralsupplies.co.za",
     "password": "Admin123!"
   }
   ```
4. Click "Execute"
5. Copy the token from the response
6. Click "Authorize" button at the top
7. Enter: `Bearer <your-token-here>`
8. Now you can test all protected endpoints!

## 7. Update Your Angular App

Update your Angular environment configuration to point to the new backend:

**src/environments/environment.ts**:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api'
};
```

**src/environments/environment.prod.ts**:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-production-domain.com/api'
};
```

## Troubleshooting

### Port Already in Use
If port 5000 or 5001 is already in use, edit `Properties/launchSettings.json` to use different ports.

### MongoDB Connection Failed
- Verify your MongoDB connection string
- Check if your IP is whitelisted (for MongoDB Atlas)
- Ensure MongoDB service is running (for local installations)

### CORS Errors from Angular
Make sure your Angular app URL is included in the CORS policy in `Program.cs`.

## Next Steps

1. **Change default admin password** via the admin panel
2. **Configure CORS** for your production domain
3. **Set up proper environment variables** for production
4. **Review and test** all API endpoints
5. **Deploy** to your hosting provider

## Running in Production

### Using Azure App Service:

```bash
# Build and publish
dotnet publish -c Release -o ./publish

# Deploy to Azure (requires Azure CLI)
az webapp deploy --resource-group <your-rg> --name <your-app-name> --src-path ./publish
```

### Using Docker:

```bash
# Build Docker image
docker build -t safuneralsupplies-api .

# Run container
docker run -p 5000:80 -e MONGODB_URI="your-connection-string" -e JWT_SECRET="your-secret" safuneralsupplies-api
```

## Development Tips

- Use `dotnet watch run` for hot reload during development
- Swagger UI is available at the root URL in development mode
- Check logs in the console for debugging information
- Use Postman or Insomnia as alternatives to Swagger for API testing

## Support

For issues or questions, refer to the main README.md or contact the development team.
