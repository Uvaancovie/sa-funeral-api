# SA Funeral Supplies E-Commerce System

A modern full-stack e-commerce platform for wholesale funeral supplies.

## Tech Stack Overview

### Backend (.NET 8 Web API)

- **Framework:** ASP.NET Core Web API (.NET 8)
- **Database:** PostgreSQL (hosted on Supabase)
- **ORM:** Entity Framework Core
- **Authentication:** JWT Bearer tokens with Role-based Auth (Admin/Customer)
- **Key Features:** Product Catalog CRUD, Order Management, Customer Approval Workflow, Audit Logging, Wishlists.

### Frontend (Angular 18)

- **Framework:** Angular 18 (Standalone Components, Signals)
- **Styling:** Tailwind CSS + Vanilla CSS (`index.css`)
- **State Management:** Angular Signals + Services
- **Routing:** Angular Router with Auth/Admin Guards
- **Key Features:** Dynamic Catalog Filtering (Category/Style/Finish/Collection), Cart System, Print/PDF Generation for Quotes/Invoices.

---

## Core Systems & Context (For Developers/LLMs)

### 1. Database Schema (Entity Framework)

The backend uses EF Core connected to a Supabase PostgreSQL instance.

- `Products`: Base catalog items. Images, Color Variations, and Features are stored as JSON arrays in text columns.
- `Orders`: Stores all orders. Order items (line items) are denormalized and stored directly inside the `Order.Items` column as a JSON array (`[{ productId, productName, variant, quantity }]`). Uses `camelCase` JSON normalization on both front and backend.
- `Users`: Handles auth. Customers require Admin approval (`status="approved"`) before placing orders.
- `Wishlists`: User-product mapping.
- `AuditLogs` / `ProductAuditLogs`: Tracks admin actions and product changes.

### 2. Angular Architecture

- **State:** Minimal RxJS; heavy reliance on Angular 16+ Signals (`computed()`, `signal()`).
- **Services:** E.g., `StoreService` (Cart state), `AuthService` (JWT/localStorage), `OrdersService` (API integration).
- **Style guide:** Dark mode aesthetics (`bg-safs-dark` `#1a103c`) combined with Gold accents (`text-safs-gold` `#a89f6e`). Design prioritizes professional, luxury wholesale b2b feel.

### 3. Orders & Quoting Workflow

1. Unauthenticated users can use the Cart to generate a PDF Quote (handled locally via window.print) or use an email template.
2. Authenticated `approved` customers can **Place Order**, generating a real order in the DB with status `pending`.
3. Customers view their history at `/orders`, which includes PDF generation.
4. Admins manage all orders at `/admin/orders`, moving statuses through `pending` -> `confirmed` -> `processing` -> `fulfilled`.

### 4. Catalog Filtering

The `/catalog` page uses heavy local filtering (via Signals) of a cached API response. Products are filtered down by:

- Category (Casket, Equipment, etc.)
- Style (Dome, Halfview, Coffin, etc.)
- Finish/Color (Cherry, Mahogany, Pecan, etc.)
- Collection (e.g., 2026 Collection flag for new imports)

### 5. Known Gotchas

- **JSON Serialization:** EF Core text columns holding JSON. The C# model uses custom getters/setters or direct serialization. Ensure C# uses `JsonNamingPolicy.CamelCase` so the Angular frontend (`parseItems()`, `parseColorVariations()`) doesn't fail on casing discrepancies.
- **Tailwind:** Processed via Angular builder. Global styles in `styles.css` / `index.css`.
- **Migrations:** Since the DB is on Supabase, `dotnet ef` CLI might occasionally fail if SSL strictness causes design-time errors. In such cases, run direct SQL via script (e.g., `create-orders-table.js`).

---

## Getting Started

### Backend Setup

1. In `sa-funeral-api/`, ensure `appsettings.json` has the correct `Supabase:ConnectionString` and `Jwt:Secret`.
2. Run `dotnet restore`
3. Run `dotnet run` (Starts API on `http://localhost:5038` and `https://localhost:7084`)

### Frontend Setup

1. `cd sa-funerals-catalog`
2. `npm install`
3. `ng serve` or `npm run dev` (Starts Angular dev server on `http://localhost:4200`)
*(Note: A proxy is configured in Angular so `/api/*` proxies to the .NET backend during dev).*
