# Migration Guide: Vercel to .NET API

This guide helps you transition from the Vercel serverless backend to the new .NET API.

## Why Migrate?

- **Better Performance**: Native .NET performance vs serverless cold starts
- **More Control**: Full control over hosting and scaling
- **Cost Effective**: No serverless function invocation costs
- **Enterprise Ready**: Better suited for production workloads
- **Local Development**: Easier to run and debug locally

## Migration Checklist

### 1. Update Environment Variables

**Old (Vercel `.env.local`)**:
```env
MONGODB_URI=mongodb+srv://...
JWT_SECRET=your-secret
```

**New (.NET `.env`)**:
```env
MONGODB_URI=mongodb+srv://...
JWT_SECRET=your-secret
```

✅ The environment variable names are the same!

### 2. Update API Base URL

Update your Angular application's environment configuration:

**src/environments/environment.ts**:
```typescript
// OLD
export const environment = {
  production: false,
  apiUrl: 'http://localhost:3000/api'  // Vercel dev server
};

// NEW
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api'  // .NET API
};
```

**src/environments/environment.prod.ts**:
```typescript
// OLD
export const environment = {
  production: true,
  apiUrl: 'https://your-vercel-app.vercel.app/api'
};

// NEW
export const environment = {
  production: true,
  apiUrl: 'https://your-dotnet-api-domain.com/api'
};
```

### 3. API Endpoint Mapping

All endpoints remain the same! The .NET API is a drop-in replacement:

| Endpoint | Vercel | .NET | Status |
|----------|--------|------|--------|
| Login | `POST /api/auth/login` | `POST /api/auth/login` | ✅ Same |
| Register | `POST /api/auth/register` | `POST /api/auth/register` | ✅ Same |
| List Products | `GET /api/products` | `GET /api/products` | ✅ Same |
| Get Product | `GET /api/products/[id]` | `GET /api/products/{id}` | ⚠️ URL format |
| Create Product | `POST /api/products` | `POST /api/products` | ✅ Same |
| Update Product | `PUT /api/products/[id]` | `PUT /api/products/{id}` | ⚠️ URL format |
| Delete Product | `DELETE /api/products/[id]` | `DELETE /api/products/{id}` | ⚠️ URL format |
| List Customers | `GET /api/admin/customers` | `GET /api/admin/customers` | ✅ Same |
| Create Customer | `POST /api/admin/customers` | `POST /api/admin/customers` | ✅ Same |
| Update Customer | `PATCH /api/admin/customers/[id]` | `PATCH /api/admin/customers/{id}` | ⚠️ URL format |

**Note**: The only difference is URL parameter format: `[id]` → `{id}`. However, when constructing URLs in your Angular app (like `/api/products/${id}`), there's no change needed.

### 4. Request/Response Formats

All request and response formats remain **exactly the same**. No changes needed to your Angular code!

**Example - Login Request** (No changes):
```typescript
// Your existing Angular code works as-is
this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, {
  email: 'admin@safuneralsupplies.co.za',
  password: 'Admin123!'
});
```

**Example - Get Products** (No changes):
```typescript
// Your existing Angular code works as-is
this.http.get<Product[]>(`${environment.apiUrl}/products?category=caskets`);
```

### 5. Authentication

JWT authentication works the same way:

```typescript
// Your existing interceptor works as-is
const token = localStorage.getItem('token');
const headers = new HttpHeaders({
  'Authorization': `Bearer ${token}`
});
```

### 6. Testing the Migration

#### Step 1: Start the .NET API
```bash
cd SAFuneralSuppliesAPI
dotnet run
```

#### Step 2: Update Angular environment
Change `apiUrl` to `https://localhost:5001/api`

#### Step 3: Test login
1. Run your Angular app
2. Navigate to login page
3. Login with: admin@safuneralsupplies.co.za / Admin123!
4. Verify you receive a token

#### Step 4: Test other endpoints
- Browse products
- View product details
- Access admin panel
- Create/edit customers

### 7. What to Keep from Vercel

You can keep your MongoDB database! The .NET API connects to the same MongoDB instance, so all your data remains intact.

### 8. What to Remove

Once migration is complete and tested, you can remove:

- `/api` folder (Vercel serverless functions)
- `vercel.json` configuration
- Vercel-specific environment variables in Vercel dashboard
- `api/package.json` and `api/tsconfig.json`

### 9. Deployment Options

#### Option A: Azure App Service (Recommended)
```bash
# Install Azure CLI
# Login to Azure
az login

# Create resource group
az group create --name safuneralsupplies-rg --location eastus

# Create app service plan
az appservice plan create --name safuneralsupplies-plan --resource-group safuneralsupplies-rg --sku B1 --is-linux

# Create web app
az webapp create --resource-group safuneralsupplies-rg --plan safuneralsupplies-plan --name safuneralsupplies-api --runtime "DOTNET|8.0"

# Deploy
dotnet publish -c Release -o ./publish
cd publish
zip -r ../publish.zip .
az webapp deploy --resource-group safuneralsupplies-rg --name safuneralsupplies-api --src-path ../publish.zip

# Set environment variables
az webapp config appsettings set --resource-group safuneralsupplies-rg --name safuneralsupplies-api --settings MONGODB_URI="your-connection-string" JWT_SECRET="your-secret"
```

#### Option B: Docker + Any Cloud Provider
```bash
# Build Docker image
docker build -t safuneralsupplies-api .

# Tag for your registry
docker tag safuneralsupplies-api your-registry/safuneralsupplies-api

# Push to registry
docker push your-registry/safuneralsupplies-api

# Deploy to your cloud provider (Azure, AWS, GCP, etc.)
```

#### Option C: VPS (DigitalOcean, Linode, etc.)
```bash
# Install .NET 8 on your server
# Copy published files to server
# Set up Nginx as reverse proxy
# Configure systemd service for auto-start
```

### 10. Post-Migration Checklist

- [ ] .NET API running successfully
- [ ] Angular app connects to .NET API
- [ ] Login works
- [ ] Products display correctly
- [ ] Product details page works
- [ ] Admin panel accessible
- [ ] Customer management works
- [ ] All environment variables set in production
- [ ] CORS configured for production domain
- [ ] Default admin password changed
- [ ] Monitoring/logging configured
- [ ] SSL certificate configured
- [ ] Database backup strategy in place

### 11. Rollback Plan

If you need to rollback:

1. Change `apiUrl` in Angular back to Vercel URL
2. Redeploy Angular app
3. Keep both APIs running during transition period
4. Monitor for any issues

### 12. Common Issues

#### CORS Errors
**Solution**: Update CORS policy in `Program.cs` to include your frontend domain:
```csharp
policy.WithOrigins("https://your-frontend-domain.com")
```

#### MongoDB Connection Issues
**Solution**: Verify connection string and IP whitelist in MongoDB Atlas

#### JWT Token Invalid
**Solution**: Ensure JWT_SECRET is the same in both old and new APIs during transition

#### 404 on API Calls
**Solution**: Verify `apiUrl` includes `/api` at the end: `https://yourdomain.com/api`

### 13. Benefits After Migration

✅ **Faster response times** - No cold starts  
✅ **Better debugging** - Full stack traces and logging  
✅ **Cost savings** - No per-request charges  
✅ **Easier testing** - Run entire backend locally  
✅ **Better IDE support** - Full IntelliSense in Visual Studio/Rider  
✅ **Type safety** - C# type system catches errors at compile time  
✅ **Enterprise features** - Built-in DI, logging, configuration  

### 14. Support

If you encounter any issues during migration:

1. Check the logs: `dotnet run` shows detailed error messages
2. Test with Swagger UI at `https://localhost:5001`
3. Verify environment variables are set correctly
4. Ensure MongoDB connection string is valid
5. Check that Angular `apiUrl` is correct

## Need Help?

Contact the development team or refer to:
- [README.md](README.md) - Full API documentation
- [QUICKSTART.md](QUICKSTART.md) - Quick start guide
- Swagger UI - Interactive API testing at root URL
