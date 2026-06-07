// Program.cs
// Application entry point for the Favourite Books Online Bookstore System.
// Provides interactive console menus for guest, customer, and admin users.
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using BookstoreApp.Data;
using BookstoreApp.Models;
using BookstoreApp.UI;

// ── Bootstrap ──────────────────────────────────────────────────────────────────

Console.WriteLine("╔══════════════════════════════════════════════╗");
Console.WriteLine("║       FAVOURITE BOOKS — ONLINE STORE         ║");
Console.WriteLine("║              Hawthorn, VIC                   ║");
Console.WriteLine("╚══════════════════════════════════════════════╝");

Database db = Database.GetInstance();
db.Seed();

Catalogue catalogue = new Catalogue();
foreach (Book book in db.GetAllBooks())
    catalogue.AddBook(book);

Checkout checkout = new Checkout(db, catalogue);

BookDetailUI bookDetailUI = new BookDetailUI();
CatalogueUI catalogueUI = new CatalogueUI(catalogue, bookDetailUI);

Console.WriteLine($"  System ready — {catalogue.Count} books loaded.\n");

// ── Session state ──────────────────────────────────────────────────────────────

string? currentEmail = null;
string? currentName = null;
string? currentRole = null;
string? currentAddress = null;
ShoppingCart? cart = null;

// ── Main loop ──────────────────────────────────────────────────────────────────

bool running = true;
while (running)
{
    if (currentEmail == null)
    {
        // ── Guest menu ─────────────────────────────────────────────────────────
        Console.WriteLine("\n  ╔══════════════════════════════════════╗");
        Console.WriteLine("  ║             MAIN MENU                ║");
        Console.WriteLine("  ╠══════════════════════════════════════╣");
        Console.WriteLine("  ║  1. Browse catalogue                 ║");
        Console.WriteLine("  ║  2. Login                            ║");
        Console.WriteLine("  ║  3. Register                         ║");
        Console.WriteLine("  ║  0. Exit                             ║");
        Console.WriteLine("  ╚══════════════════════════════════════╝");
        Console.Write("  Choose an option: ");

        string? choice = Console.ReadLine()?.Trim();
        Console.WriteLine();

        switch (choice)
        {
            case "1":
                catalogueUI.Show();
                break;

            case "2":
                (currentEmail, currentName, currentRole, currentAddress) = Login(db);
                if (currentEmail != null)
                {
                    Console.WriteLine($"\n  [✓] Welcome back, {currentName}! ({currentRole})");
                    if (currentRole == "Customer")
                        cart = new ShoppingCart();
                }
                break;

            case "3":
                (currentEmail, currentName, currentRole, currentAddress) = Register(db);
                if (currentEmail != null)
                {
                    Console.WriteLine($"\n  [✓] Account created. Welcome, {currentName}!");
                    cart = new ShoppingCart();
                }
                break;

            case "0":
                running = false;
                Console.WriteLine("\n  Thank you for visiting Favourite Books. Goodbye!");
                break;

            default:
                Console.WriteLine("  [!] Invalid option. Please try again.");
                break;
        }
    }
    else if (currentRole == "Customer")
    {
        // ── Customer menu ──────────────────────────────────────────────────────
        Console.WriteLine($"\n  ╔══════════════════════════════════════╗");
        Console.WriteLine($"  ║  Hello, {currentName,-29}║");
        Console.WriteLine($"  ╠══════════════════════════════════════╣");
        Console.WriteLine($"  ║  1. Browse catalogue                 ║");
        Console.WriteLine($"  ║  2. Shopping cart & place order      ║");
        Console.WriteLine($"  ║  3. My account                       ║");
        Console.WriteLine($"  ║  4. Logout                           ║");
        Console.WriteLine($"  ║  0. Exit                             ║");
        Console.WriteLine($"  ╚══════════════════════════════════════╝");
        Console.Write("  Choose an option: ");

        string? choice = Console.ReadLine()?.Trim();
        Console.WriteLine();

        switch (choice)
        {
            case "1":
                catalogueUI.Show();
                break;

            case "2":
                new CartAndOrderUI(currentEmail, currentAddress ?? "",
                                   cart!, catalogue, checkout, db).Show();
                break;

            case "3":
                currentAddress = MyAccount(currentEmail, currentName!, currentAddress ?? "", db);
                break;

            case "4":
                Console.WriteLine($"  Goodbye, {currentName}!");
                currentEmail = currentName = currentRole = currentAddress = null;
                cart = null;
                break;

            case "0":
                running = false;
                Console.WriteLine("\n  Thank you for visiting Favourite Books. Goodbye!");
                break;

            default:
                Console.WriteLine("  [!] Invalid option.");
                break;
        }
    }
    else if (currentRole == "Administrator" || currentRole == "StoreManager")
    {
        // ── Admin menu ─────────────────────────────────────────────────────────
        Console.WriteLine($"\n  ╔══════════════════════════════════════╗");
        Console.WriteLine($"  ║  Admin: {currentName,-29}║");
        Console.WriteLine($"  ╠══════════════════════════════════════╣");
        Console.WriteLine($"  ║  1. Browse catalogue                 ║");
        Console.WriteLine($"  ║  2. Manage catalogue  [Lakshit]      ║");
        Console.WriteLine($"  ║  3. View all orders                  ║");
        Console.WriteLine($"  ║  4. Sales Report                     ║");
        Console.WriteLine($"  ║  5. Logout                           ║");
        Console.WriteLine($"  ║  0. Exit                             ║");
        Console.WriteLine($"  ╚══════════════════════════════════════╝");
        Console.Write("  Choose an option: ");

        string? choice = Console.ReadLine()?.Trim();
        Console.WriteLine();

        switch (choice)
        {
            case "1":
               catalogueUI.Show();
                break;

            case "2":
                // Stub — to be implemented by Lakshit (AdminCatalogueUI)
                Console.WriteLine("  [Admin] Catalogue management — Lakshit's module (coming soon).");
                break;

            case "3":
                ViewAllOrders(db);
                break;
    
            case "4":
                new SalesReportUI(db).Show();
                break;

            case "5":
                Console.WriteLine($"  Goodbye, {currentName}!");
                currentEmail = currentName = currentRole = currentAddress = null;
                break;

            case "0":
                running = false;
                Console.WriteLine("\n  Thank you for visiting Favourite Books. Goodbye!");
                break;

            default:
                Console.WriteLine("  [!] Invalid option.");
                break;
        }
    }
}

// ── Browse Catalogue (Aidan's module stub — basic version included here) ───────

//static void BrowseCatalogue(Catalogue catalogue)
//{
//    bool running = true;
//    while (running)
//    {
//        Console.WriteLine("\n  ╔══════════════════════════════════════╗");
//        Console.WriteLine("  ║          BROWSE CATALOGUE            ║");
//        Console.WriteLine("  ╠══════════════════════════════════════╣");
//        Console.WriteLine("  ║  1. View all books                   ║");
//        Console.WriteLine("  ║  2. Search by keyword                ║");
//        Console.WriteLine("  ║  3. Filter by genre                  ║");
//        Console.WriteLine("  ║  4. Filter by max price              ║");
//        Console.WriteLine("  ║  5. View book details (by ISBN)      ║");
//        Console.WriteLine("  ║  0. Back                             ║");
//        Console.WriteLine("  ╚══════════════════════════════════════╝");
//        Console.Write("  Choose an option: ");

//        string? input = Console.ReadLine()?.Trim();
//        Console.WriteLine();

//        switch (input)
//        {
//            case "1":
//                PrintBookList(catalogue.ListAvailableBooks());
//                break;

//            case "2":
//                Console.Write("  Enter keyword (title / author / genre): ");
//                string? kw = Console.ReadLine()?.Trim();
//                var results = catalogue.SearchBooks(kw ?? "");
//                Console.WriteLine($"\n  {results.Count} result(s) found:");
//                PrintBookList(results);
//                break;

//            case "3":
//                Console.Write("  Enter genre: ");
//                string? genre = Console.ReadLine()?.Trim();
//                var genreResults = catalogue.FilterByGenre(genre ?? "");
//                Console.WriteLine($"\n  {genreResults.Count} result(s) found:");
//                PrintBookList(genreResults);
//                break;

//            case "4":
//                Console.Write("  Enter maximum price ($): ");
//                if (!decimal.TryParse(Console.ReadLine()?.Trim(), out decimal max))
//                {
//                    Console.WriteLine("  [!] Invalid price.");
//                    break;
//                }
//                var priceResults = catalogue.FilterByMaxPrice(max);
//                Console.WriteLine($"\n  {priceResults.Count} result(s) found:");
//                PrintBookList(priceResults);
//                break;

//            case "5":
//                Console.Write("  Enter ISBN: ");
//                string? isbn = Console.ReadLine()?.Trim();
//                Book? book = catalogue.FindByISBN(isbn ?? "");
//                if (book == null)
//                    Console.WriteLine("  [!] Book not found.");
//                else
//                {
//                    Console.WriteLine();
//                    Console.WriteLine(book.ToDetailString());
//                }
//                break;

//            case "0":
//                running = false;
//                break;

//            default:
//                Console.WriteLine("  [!] Invalid option.");
//                break;
//        }
//    }
//}

static void PrintBookList(List<Book> books)
{
    if (books.Count == 0)
    {
        Console.WriteLine("  No books found.");
        return;
    }
    Console.WriteLine($"\n  {"ISBN",-25} {"Title",-30} {"Author",-22} {"Price",7}");
    Console.WriteLine($"  {"────",-25} {"─────",-30} {"──────",-22} {"─────",7}");
    foreach (Book b in books)
        Console.WriteLine($"  {b.ISBN,-25} {b.Title,-30} {b.Author,-22} ${b.Price,6:F2}");
}

// ── Login ──────────────────────────────────────────────────────────────────────

static (string? email, string? name, string? role, string? address) Login(Database db)
{
    Console.WriteLine("  === LOGIN ===");
    Console.Write("  Email   : ");
    string? email = Console.ReadLine()?.Trim();

    Console.Write("  Password: ");
    string? password = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        Console.WriteLine("  [!] Email and password are required.");
        return (null, null, null, null);
    }

    var user = db.GetUserByCredentials(email, password);
    if (user == null)
    {
        Console.WriteLine("  [!] Incorrect email or password.");
        if (!db.EmailExists(email))
            Console.WriteLine("  Tip: This email is not registered. Use option 3 to register.");
        return (null, null, null, null);
    }

    return (user.Value.Email, user.Value.Name, user.Value.Role, user.Value.Address);
}

// ── Register ───────────────────────────────────────────────────────────────────

static (string? email, string? name, string? role, string? address) Register(Database db)
{
    Console.WriteLine("  === REGISTER ===");

    Console.Write("  Full name               : ");
    string? name = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("  [!] Name cannot be empty.");
        return (null, null, null, null);
    }

    Console.Write("  Email                   : ");
    string? email = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
    {
        Console.WriteLine("  [!] Please enter a valid email address.");
        return (null, null, null, null);
    }

    if (db.EmailExists(email))
    {
        Console.WriteLine("  [!] An account with this email already exists. Please login.");
        return (null, null, null, null);
    }

    Console.Write("  Password (min 8 chars)  : ");
    string? password = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
    {
        Console.WriteLine("  [!] Password must be at least 8 characters.");
        return (null, null, null, null);
    }

    Console.Write("  Delivery address        : ");
    string? address = Console.ReadLine()?.Trim() ?? "";

    bool saved = db.SaveUser(email, password, "Customer", name, address);
    if (!saved)
    {
        Console.WriteLine("  [!] Registration failed.");
        return (null, null, null, null);
    }

    return (email.ToLower(), name, "Customer", address);
}

// ── My Account ─────────────────────────────────────────────────────────────────

static string MyAccount(string email, string name, string address, Database db)
{
    // Stub — to be fully implemented by Joel (AccountUI)
    // Basic version included here so the customer menu works
    bool running = true;
    string currentAddress = address;

    while (running)
    {
        Console.WriteLine("\n  ╔══════════════════════════════════════╗");
        Console.WriteLine("  ║            MY ACCOUNT                ║");
        Console.WriteLine("  ╠══════════════════════════════════════╣");
        Console.WriteLine("  ║  1. View account details             ║");
        Console.WriteLine("  ║  2. Update delivery address          ║");
        Console.WriteLine("  ║  0. Back                             ║");
        Console.WriteLine("  ╚══════════════════════════════════════╝");
        Console.Write("  Choose an option: ");

        string? input = Console.ReadLine()?.Trim();
        Console.WriteLine();

        switch (input)
        {
            case "1":
                Console.WriteLine($"  Name    : {name}");
                Console.WriteLine($"  Email   : {email}");
                Console.WriteLine($"  Address : {(string.IsNullOrWhiteSpace(currentAddress) ? "(not set)" : currentAddress)}");
                break;

            case "2":
                Console.Write("  New delivery address: ");
                string? newAddr = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(newAddr))
                {
                    currentAddress = newAddr;
                    Console.WriteLine("  [✓] Address updated.");
                }
                else
                {
                    Console.WriteLine("  [!] Address cannot be empty.");
                }
                break;

            case "0":
                running = false;
                break;

            default:
                Console.WriteLine("  [!] Invalid option.");
                break;
        }
    }

    return currentAddress;
}

// ── View All Orders (Admin) ────────────────────────────────────────────────────

static void ViewAllOrders(Database db)
{
    Console.WriteLine("  === ALL ORDERS ===");
    List<Order> orders = db.GetAllOrders();

    if (orders.Count == 0)
    {
        Console.WriteLine("  No orders in the system yet.");
        return;
    }

    foreach (Order order in orders)
        order.DisplaySummary();
}
