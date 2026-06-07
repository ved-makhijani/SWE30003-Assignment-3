# Favourite Books — Online Bookstore System

**SWE30003 Software Architectures and Design — Assignment 3**
Swinburne University of Technology | Semester 1, 2026 | Group Wed 16:30 6

---

## Overview

Favourite Books is an online bookstore system built for a local Hawthorn bookstore. The system allows customers to browse books, manage a shopping cart, place orders, and track order history. Administrators can manage the book catalogue and view sales reports.

The system is implemented as a **Blazor Server** web application running on .NET 10.

---

## Group Members

| Student ID | Name | Responsibility |
|---|---|---|
| 104762184 | Ved Jay Makhijani | Core domain models, cart, checkout, database, unit tests, Blazor UI conversion |
| 105036550 | Aidan Tilley | UML diagram, catalogue UI, UI framework |
| 103476303 | Joel Hadfield | Auth classes, account management UI |
| 105028858 | Lakshit Bansal | Admin catalogue UI, sales report, scenarios, report |

---

## Tech Stack

| Item | Detail |
|---|---|
| Language | C# 12 |
| Framework | .NET 10.0 |
| UI | Blazor Server |
| Testing | MSTest |
| Storage | In-memory (no external database required) |
| IDE | Visual Studio Code with C# Dev Kit |
| Platform tested on | macOS (Apple Silicon, arm64) |

---

## Project Structure

```
SWE30003-Assignment-3/
├── BookStoreApp/                   # Main Blazor Server application
│   ├── Models/                     # Domain model classes
│   │   ├── Book.cs
│   │   ├── Catalogue.cs
│   │   ├── CartItem.cs
│   │   ├── ShoppingCart.cs
│   │   ├── OrderLine.cs
│   │   ├── Order.cs
│   │   ├── Invoice.cs
│   │   └── Checkout.cs
│   ├── Data/
│   │   └── Database.cs             # Singleton in-memory data store
│   ├── Auth/
│   │   └── User.cs                 # User, Customer, Administrator, StoreManager, WarehouseStaff
│   ├── Services/
│   │   └── SessionService.cs       # Scoped session state (login, cart, role)
│   ├── Pages/                      # Blazor pages (one per business area)
│   │   ├── Index.razor             # Home page
│   │   ├── Catalogue.razor         # Browse, search, filter books + book detail + add to cart
│   │   ├── Login.razor             # Login
│   │   ├── Register.razor          # Register new account
│   │   ├── Cart.razor              # Shopping cart + checkout + invoice
│   │   ├── Orders.razor            # Customer order history
│   │   ├── Account.razor           # Manage account details
│   │   ├── AdminCatalogue.razor    # Admin: add, edit, disable books + view all orders
│   │   ├── SalesReport.razor       # Admin: revenue, order count, export
│   │   ├── _Host.cshtml            # Blazor Server host page
│   │   └── _Layout.cshtml          # HTML shell
│   ├── Shared/
│   │   └── MainLayout.razor        # Navbar and page layout
│   ├── wwwroot/
│   │   └── css/app.css             # Application stylesheet
│   ├── Properties/
│   │   └── launchSettings.json     # Sets Development environment by default
│   ├── App.razor                   # Router
│   ├── _Imports.razor              # Global using statements
│   ├── BookStoreApp.csproj         # Project file (Microsoft.NET.Sdk.Web)
│   └── Program.cs                  # Application entry point + DI setup
│
└── BookStoreApp.Tests/             # MSTest unit test project
    ├── BookTests.cs
    ├── CatalogueTests.cs
    ├── ShoppingCartTests.cs
    ├── OrderTests.cs
    ├── InvoiceTests.cs
    └── CheckoutTests.cs
```

---

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

Verify installation:
```bash
dotnet --version
```

---

## How to Run

**1. Navigate to the project root:**
```bash
cd SWE30003-Assignment-3
```

**2. Publish the application:**
```bash
dotnet publish BookStoreApp/BookStoreApp.csproj -c Debug -o BookStoreApp/publish
```

**3. Run from the published output:**
```bash
cd BookStoreApp/publish && dotnet BookStoreApp.dll
```

**4. Open your browser and go to:**
```
http://localhost:5000
```

**To stop the server:** press `Ctrl+C`

**To run again after making changes:**
```bash
cd /path/to/SWE30003-Assignment-3
dotnet publish BookStoreApp/BookStoreApp.csproj -c Debug -o BookStoreApp/publish
cd BookStoreApp/publish && dotnet BookStoreApp.dll
```

---

## How to Run Unit Tests

From the project root:
```bash
dotnet test
```

Expected output:
```
Test summary: total: 82, failed: 0, succeeded: 82
```

---

## Default Credentials

The database is seeded automatically on every run with 8 sample books and a default admin account.

| Role | Email | Password |
|---|---|---|
| Administrator | admin@favouritebooks.com | Admin1234 |

Register a new customer account from the `/register` page.

---

## Pages and URLs

| URL | Page | Access |
|---|---|---|
| `/` | Home | Everyone |
| `/catalogue` | Browse books, search, filter, add to cart | Everyone |
| `/login` | Login | Guest only |
| `/register` | Register new account | Guest only |
| `/cart` | Shopping cart and checkout | Customer |
| `/orders` | Order history | Customer |
| `/account` | Manage account details | Customer |
| `/admin` | Manage catalogue and view all orders | Admin / StoreManager |
| `/salesreport` | Revenue stats and export | Admin / StoreManager |

---

## Features

### Customer
- Browse the full book catalogue with search (title, author, genre) and filters (genre, author, max price)
- View detailed book information including description and stock
- Register and log in to an account
- Add books to shopping cart, update quantities, remove items
- Checkout with delivery address and payment method selection (simulated)
- View full order history

### Administrator
- All customer features
- Manage catalogue: add books (duplicate ISBN validation), edit book details, disable books
- View all orders across all customers
- View sales report: total revenue, order count, average order value, order breakdown
- Export sales report to text file

---

## Architecture

The system uses two complementary architecture styles:

**Layered (N-Tier):**
```
Presentation Layer   →   Pages/*.razor, Shared/MainLayout.razor
Application Layer    →   Program.cs (DI), Services/SessionService.cs
Domain Layer         →   Models/ (Book, Catalogue, ShoppingCart, Order, Invoice, Checkout)
Data Layer           →   Data/Database.cs
```

**MVC within Presentation/Application layers:**
- **Model** — domain classes in `Models/` and `Data/`
- **View** — Razor components in `Pages/` and `Shared/`
- **Controller** — `SessionService`, `Checkout`, ASP.NET Core routing in `Program.cs`

---

## Coding Standard

Microsoft C# Coding Conventions:
https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

---

## Notes

- Payment processing is simulated as per Assignment 3 specification — no real payment gateway is used
- Data is stored in-memory and resets on each application restart
- The application must be run from the published output folder for static files to load correctly
- Developed and tested on macOS (Apple Silicon) with .NET 10.0
