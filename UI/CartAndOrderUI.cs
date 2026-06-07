// CartAndOrderUI.cs
// Console UI for shopping cart management and order placement.
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using BookstoreApp.Data;
using BookstoreApp.Models;

namespace BookstoreApp.UI
{
    /// <summary>
    /// Provides the console interface for managing the shopping cart,
    /// placing orders, and viewing order history.
    /// </summary>
    public class CartAndOrderUI
    {
        private readonly ShoppingCart _cart;
        private readonly Catalogue _catalogue;
        private readonly Checkout _checkout;
        private readonly Database _database;
        private readonly string _customerEmail;
        private string _deliveryAddress;

        public CartAndOrderUI(string customerEmail, string deliveryAddress,
                              ShoppingCart cart, Catalogue catalogue,
                              Checkout checkout, Database database)
        {
            _customerEmail = customerEmail;
            _deliveryAddress = deliveryAddress;
            _cart = cart;
            _catalogue = catalogue;
            _checkout = checkout;
            _database = database;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n  ╔══════════════════════════════════════╗");
                Console.WriteLine("  ║         SHOPPING CART MENU           ║");
                Console.WriteLine("  ╠══════════════════════════════════════╣");
                Console.WriteLine("  ║  1. View cart                        ║");
                Console.WriteLine("  ║  2. Add book to cart (by ISBN)       ║");
                Console.WriteLine("  ║  3. Update item quantity             ║");
                Console.WriteLine("  ║  4. Remove item from cart            ║");
                Console.WriteLine("  ║  5. Proceed to checkout              ║");
                Console.WriteLine("  ║  6. View my order history            ║");
                Console.WriteLine("  ║  0. Back to main menu                ║");
                Console.WriteLine("  ╚══════════════════════════════════════╝");
                Console.Write("  Choose an option: ");

                string? input = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (input)
                {
                    case "1": ViewCart(); break;
                    case "2": AddBookToCart(); break;
                    case "3": UpdateQuantity(); break;
                    case "4": RemoveFromCart(); break;
                    case "5": ProceedToCheckout(); break;
                    case "6": ViewOrderHistory(); break;
                    case "0": running = false; break;
                    default:
                        Console.WriteLine("  [!] Invalid option. Please try again.");
                        break;
                }
            }
        }

        // ── View Cart ─────────────────────────────────────────────────────────

        private void ViewCart()
        {
            Console.WriteLine("  === YOUR SHOPPING CART ===");
            _cart.DisplayContents();
        }

        // ── Add Book ──────────────────────────────────────────────────────────

        private void AddBookToCart()
        {
            Console.WriteLine("  === ADD BOOK TO CART ===");

            // Show available books first so user knows ISBNs
            List<Book> books = _catalogue.ListAvailableBooks();
            if (books.Count == 0)
            {
                Console.WriteLine("  [!] No books currently available.");
                return;
            }

            Console.WriteLine($"\n  {"ISBN",-25} {"Title",-30} {"Author",-20} {"Price",7}");
            Console.WriteLine($"  {"────",-25} {"─────",-30} {"──────",-20} {"─────",7}");
            foreach (Book b in books)
                Console.WriteLine($"  {b.ISBN,-25} {b.Title,-30} {b.Author,-20} ${b.Price,6:F2}");

            Console.Write("\n  Enter ISBN of book to add: ");
            string? isbn = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(isbn))
            {
                Console.WriteLine("  [!] ISBN cannot be empty.");
                return;
            }

            Book? book = _catalogue.FindByISBN(isbn);
            if (book == null)
            {
                Console.WriteLine($"  [!] No book found with ISBN '{isbn}'.");
                return;
            }

            Console.WriteLine($"  Found: {book.Title} by {book.Author} — ${book.Price:F2} ({book.StockQuantity} in stock)");
            Console.Write("  Enter quantity (press Enter for 1): ");
            string? qtyInput = Console.ReadLine()?.Trim();

            int quantity = 1;
            if (!string.IsNullOrWhiteSpace(qtyInput))
            {
                if (!int.TryParse(qtyInput, out quantity) || quantity <= 0)
                {
                    Console.WriteLine("  [!] Invalid quantity. Must be a positive whole number.");
                    return;
                }
            }

            bool added = _cart.AddItem(book, quantity);
            if (added)
                Console.WriteLine($"  [✓] '{book.Title}' (x{quantity}) added to cart.");
            else
                Console.WriteLine($"  [!] Could not add '{book.Title}' — insufficient stock or unavailable.");
        }

        // ── Update Quantity ───────────────────────────────────────────────────

        private void UpdateQuantity()
        {
            Console.WriteLine("  === UPDATE ITEM QUANTITY ===");

            if (_cart.IsEmpty)
            {
                Console.WriteLine("  [!] Your cart is empty.");
                return;
            }

            _cart.DisplayContents();

            Console.Write("  Enter ISBN of item to update: ");
            string? isbn = Console.ReadLine()?.Trim();

            Console.Write("  Enter new quantity: ");
            string? qtyInput = Console.ReadLine()?.Trim();

            if (!int.TryParse(qtyInput, out int newQty) || newQty <= 0)
            {
                Console.WriteLine("  [!] Invalid quantity. Must be a positive whole number.");
                return;
            }

            bool updated = _cart.UpdateQuantity(isbn ?? "", newQty);
            if (updated)
                Console.WriteLine("  [✓] Quantity updated.");
            else
                Console.WriteLine("  [!] Could not update — check the ISBN and quantity.");
        }

        // ── Remove From Cart ──────────────────────────────────────────────────

        private void RemoveFromCart()
        {
            Console.WriteLine("  === REMOVE ITEM FROM CART ===");

            if (_cart.IsEmpty)
            {
                Console.WriteLine("  [!] Your cart is empty.");
                return;
            }

            _cart.DisplayContents();

            Console.Write("  Enter ISBN of item to remove: ");
            string? isbn = Console.ReadLine()?.Trim();

            bool removed = _cart.RemoveItem(isbn ?? "");
            if (removed)
            {
                Console.WriteLine("  [✓] Item removed.");
                if (_cart.IsEmpty)
                    Console.WriteLine("  Your cart is now empty.");
            }
            else
            {
                Console.WriteLine("  [!] Item not found in cart.");
            }
        }

        // ── Checkout ──────────────────────────────────────────────────────────

        private void ProceedToCheckout()
        {
            Console.WriteLine("  === CHECKOUT ===");

            if (_cart.IsEmpty)
            {
                Console.WriteLine("  [!] Your cart is empty. Add books before checking out.");
                return;
            }

            // Show order summary
            Console.WriteLine("\n  Order Summary:");
            _cart.DisplayContents();

            // Confirm or enter delivery address
            string deliveryAddress = ConfirmDeliveryAddress();
            if (string.IsNullOrWhiteSpace(deliveryAddress))
            {
                Console.WriteLine("  [!] Checkout cancelled — no delivery address provided.");
                return;
            }

            // Final confirmation
            Console.WriteLine($"\n  Deliver to : {deliveryAddress}");
            Console.WriteLine($"  Order total: ${_cart.CalculateTotal():F2}");
            Console.Write("\n  Confirm order? (y/n): ");
            string? confirm = Console.ReadLine()?.Trim().ToLower();

            if (confirm != "y")
            {
                Console.WriteLine("  [!] Order cancelled. Your cart has been kept.");
                return;
            }

            // Process checkout
            Invoice? invoice = _checkout.ProcessCheckout(_cart, _customerEmail, deliveryAddress);

            if (invoice == null)
            {
                Console.WriteLine("  [!] Checkout failed — one or more items may be out of stock.");
                return;
            }

            // Display invoice
            invoice.Display();

            // Payment stub
            Console.WriteLine("\n  ── PAYMENT ──────────────────────────────────");
            Console.WriteLine("  Select payment method:");
            Console.WriteLine("    1. Credit / Debit Card");
            Console.WriteLine("    2. PayPal");
            Console.Write("  Choice: ");
            string? payChoice = Console.ReadLine()?.Trim();

            if (payChoice != "1" && payChoice != "2")
            {
                Console.WriteLine("  [!] Invalid payment method. Order saved as Pending Payment.");
                return;
            }

            string method = payChoice == "1" ? "Credit/Debit Card" : "PayPal";
            Console.WriteLine($"\n  Processing payment via {method}...");
            Console.WriteLine("  (Enter any details and press Enter to simulate payment)");
            Console.ReadLine();

            // Process payment stub
            _checkout.ProcessPaymentStub(invoice.Order, invoice);

            Console.WriteLine("\n  ── PAYMENT CONFIRMED ────────────────────────");
            invoice.Display();
            Console.WriteLine("\n  [✓] Thank you for your order!");
        }

        private string ConfirmDeliveryAddress()
        {
            if (!string.IsNullOrWhiteSpace(_deliveryAddress))
            {
                Console.WriteLine($"\n  Saved address: {_deliveryAddress}");
                Console.Write("  Use this address? (y/n): ");
                string? use = Console.ReadLine()?.Trim().ToLower();
                if (use == "y") return _deliveryAddress;
            }
            else
            {
                Console.WriteLine("  No saved delivery address.");
            }

            Console.Write("  Enter delivery address: ");
            string? address = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(address))
                return string.Empty;

            // Save it for future orders this session
            _deliveryAddress = address;
            return address;
        }

        // ── Order History ─────────────────────────────────────────────────────

        private void ViewOrderHistory()
        {
            Console.WriteLine("  === YOUR ORDER HISTORY ===");
            List<Order> orders = _database.GetOrdersByCustomer(_customerEmail);

            if (orders.Count == 0)
            {
                Console.WriteLine("  No orders found.");
                return;
            }

            foreach (Order order in orders)
                order.DisplaySummary();
        }
    }
}
