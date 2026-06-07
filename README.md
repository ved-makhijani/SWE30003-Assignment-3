# Favourite Books — Online Bookstore System

**SWE30003 Software Architectures and Design — Assignment 3**
Swinburne University of Technology | Semester 1, 2026 | Group Wed 16:30 6

---

## Overview

Favourite Books is an online bookstore system built for a local Hawthorn bookstore. The system allows customers to browse books, manage a shopping cart, place orders, and track order history. Administrators can manage the book catalogue and view sales reports.

The system was originally implemented as a console application and converted to a **Blazor Server** web application for Assignment 3.

---

## Group Members

| Student ID | Name | Responsibility |
|---|---|---|
| 104762184 | Ved Jay Makhijani | Core domain models, cart, checkout, database, unit tests |
| 105036550 | Aidan Tilley | UML diagram, catalogue UI, UI framework |
| 103476303 | Joel Hadfield | Auth classes, account management UI |
| 105028858 | Lakshit Bansal | Admin catalogue UI, sales report, scenarios |

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
│   │   └── SessionService.cs       # Scoped session state (replaces console session variables)
│   ├── Pages/                      # Blazor pages (one per business area)
│   │   ├── Index.razor             # Home page
│   │   ├── Catalogue.razor         # Browse, search, filter books + book detail
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
│   ├── App.razor                   # Router
│   ├── _Imports.razor              # Global using statements
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

**1. Clone or download the project**

**2. Navigate to the project root:**
```bash
cd SWE30003-Assignment-3
```

**3. Build the project:**
```bash
dotnet build
```

**4. Run the Blazor web application:**
```bash
dotnet run --project BookStoreApp/BookStoreApp.csproj
```

**5. Open your browser and go to:**
```
http://localhost:5000
```

---

## How to Run Tests

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

The database is seeded automatically on first run with sample books and a default admin account.

| Role | Email | Password |
|---|---|---|
| Administrator | admin@favouritebooks.com | Admin1234 |

You can register a new customer account from the Register page.

---

## Features

### Customer
- Browse the book catalogue (search by keyword, filter by genre, author, price)
- View detailed book information
- Register and log in to an account
- Add books to shopping cart, update quantities, remove items
- Proceed to checkout with delivery address and payment method selection
- View order history

### Admin / Store Manager
- All customer features
- Manage catalogue: add books (with duplicate ISBN validation), edit book details, disable books
- View all orders across all customers
- View sales report: total revenue, order count, average order value
- Export sales report to text file

---

## Architecture

The system uses two complementary architecture styles:

**Layered (N-Tier):**
```
Presentation Layer   →   Pages/*.razor, Shared/MainLayout.razor
Application Layer    →   Program.cs, Services/SessionService.cs
Domain Layer         →   Models/ (Book, Catalogue, ShoppingCart, Order, Invoice, Checkout)
Data Layer           →   Data/Database.cs
```

**MVC within the Presentation/Application layers:**
- **Model** — domain classes in `Models/` and `Data/`
- **View** — Razor components in `Pages/` and `Shared/`
- **Controller** — `SessionService`, `Checkout`, routing in `Program.cs`

---

## Coding Standard

Microsoft C# Coding Conventions:
https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

---

## Notes

- Payment processing is simulated (stub) as per Assignment 3 specification — no real payment gateway is used
- Data is stored in-memory and resets on each application restart
- The system was developed and tested on macOS (Apple Silicon) with .NET 10.0
