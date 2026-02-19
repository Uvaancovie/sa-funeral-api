# 🚀 SA Funeral Supplies API - SQL Server Edition

## ✅ What's Changed

Your API now uses **SQL Server** instead of MongoDB! All your data will be stored in the `sa-funerals` database on your local SQL Server.

## 🎯 Quick Start

### 1. Restore Dependencies
```powershell
dotnet restore
```

### 2. Create Database & Apply Migrations
```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Run the API
```powershell
dotnet run
```

### 4. Open Swagger UI
Navigate to: **http://localhost:5000** or **https://localhost:5001**

You'll see a beautiful, interactive API documentation interface!

## 🔐 Login & Test

### Step 1: Login
1. In Swagger UI, find **POST /api/auth/login**
2. Click "Try it out"
3. Use these credentials:
   ```json
   {
     "email": "admin@safuneralsupplies.co.za",
     "password": "Admin123!"
   }
   ```
4. Click "Execute"
5. **Copy the token** from the response

### Step 2: Authorize
1. Click the **🔒 Authorize** button at the top right
2. Paste your token (just the token, not "Bearer")
3. Click "Authorize"
4. Click "Close"

### Step 3: Test Endpoints!
Now you can test all endpoints:
- ✅ GET /api/products - View all products
- ✅ POST /api/products - Create a product (Admin)
- ✅ GET /api/admin/ customers - View customers (Admin)
- ✅ POST /api/admin/customers - Create a customer (Admin)

## 📊 Your Database

**Connection String:**
```
Data Source=LAPTOP-A5DOSJNT
Initial Catalog=sa-funerals
Integrated Security=True
Trust Server Certificate=True
```

**Tables Created:**
- `Users` - Admin and customer accounts
- `Products` - Product catalog

**Default Admin Account (automatically created):**
- Email: admin@safuneralsupplies.co.za
- Password: Admin123!

## 🛠️ Useful Commands

### View Database in SQL Server Management Studio (SSMS)
1. Connect to: `LAPTOP-A5DOSJNT`
2. Find database: `sa-funerals`
3. Browse tables: `Users`, `Products`

### Create New Migration (after model changes)
```powershell
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

### Reset Database (WARNING: Deletes all data!)
```powershell
dotnet ef database drop -f
dotnet ef database update
```

### Run with Auto-Reload (for development)
```powershell
dotnet watch run
```

## 🎨 Swagger UI Features

Your enhanced Swagger UI includes:

- ✅ **Interactive Testing** - Test all endpoints right in the browser
- ✅ **JWT Authentication** - One-click authorize button
- ✅ **Request Examples** - See example requests for every endpoint
- ✅ **Response Schemas** - Know exactly what to expect
- ✅ **Grouped Endpoints** - Organized by category
- ✅ **Search & Filter** - Find endpoints quickly
- ✅ **Dark Mode Support** - Easy on the eyes
- ✅ **Request Duration** - Monitor performance
- ✅ **Deep Linking** - Share specific endpoint URLs

## 📝 API Endpoints

### Public Endpoints (No Auth Required)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login with email/password |
| GET | `/api/products` | List all products |
| GET | `/api/products/{id}` | Get single product |

### Admin Only Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |
| GET | `/api/admin/customers` | List customers |
| POST | `/api/admin/customers` | Create customer |
| PATCH | `/api/admin/customers/{id}` | Update customer status |

## 🔧 Configuration

Edit `appsettings.json` to change:
- Connection string
- JWT secret (change for production!)
- CORS allowed origins
- Logging levels

## 🐛 Troubleshooting

### "Cannot connect to SQL Server"
✅ **Solution:** Ensure SQL Server is running
```powershell
# Check SQL Server service status
Get-Service -Name MSSQL*
```

### "Login failed for user"
✅ **Solution:** Verify Integrated Security is enabled or add credentials to connection string

### "Table 'Users' does not exist"
✅ **Solution:** Run migrations
```powershell
dotnet ef database update
```

### "JWT Secret not configured"
✅ **Solution:** Check `appsettings.json` has a JWT:Secret value (at least 32 characters)

### Swagger UI not loading
✅ **Solution:** Make sure you're navigating to the root URL: http://localhost:5000/

## 🚀 Next Steps

1. ✅ **Test the API** using Swagger UI
2. ✅ **Create some products** via POST /api/products
3. ✅ **Add customers** via POST /api/admin/customers
4. ✅ **Update your Angular app** to use the new API
5. ✅ **Deploy to production** when ready

## 📚 Additional Resources

- **Entity Framework Core Docs:** https://docs.microsoft.com/en-us/ef/core/
- **ASP.NET Core Docs:** https://docs.microsoft.com/en-us/aspnet/core/
- **Swagger/OpenAPI:** https://swagger.io/docs/

## 🎉 That's It!

You now have a fully functional .NET API with:
- ✅ SQL Server database
- ✅ Entity Framework Core
- ✅ JWT authentication
- ✅ Interactive Swagger UI
- ✅ Role-based authorization
- ✅ Clean architecture

**Enjoy building! 🚀**
