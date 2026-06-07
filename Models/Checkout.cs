// Checkout.cs
// Coordinates the checkout process: validates cart, creates Order and Invoice,
// deducts stock, persists results, and provides a payment stub.
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using BookstoreApp.Data;

namespace BookstoreApp.Models
{
    /// <summary>
    /// Handles the full checkout flow for a customer.
    /// Validates stock, creates a confirmed Order and Invoice,
    /// deducts stock, and persists both to the Database.
    /// </summary>
    public class Checkout
    {
        private readonly Database _database;
        private readonly Catalogue _catalogue;

        public Checkout(Database database, Catalogue catalogue)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _catalogue = catalogue ?? throw new ArgumentNullException(nameof(catalogue));
        }

        // ── Main checkout flow ────────────────────────────────────────────────

        /// <summary>
        /// Processes a full checkout. Returns the generated Invoice on success,
        /// or null if validation fails (empty cart, insufficient stock, etc.).
        /// On success: creates Order + Invoice, deducts stock, clears cart.
        /// </summary>
        public Invoice? ProcessCheckout(ShoppingCart cart, string customerEmail,
                                        string deliveryAddress)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));

            if (cart.IsEmpty)
                return null;

            if (string.IsNullOrWhiteSpace(deliveryAddress))
                return null;

            if (!ValidateStock(cart))
                return null;

            Order order = new Order(customerEmail, deliveryAddress, cart.Items.ToList());
            Invoice invoice = new Invoice(order);

            DeductStock(cart);

            _database.SaveOrder(order);
            _database.SaveInvoice(invoice);

            cart.Clear();

            return invoice;
        }

        /// <summary>
        /// Payment stub — simulates a successful payment.
        /// Updates order status to Paid and marks invoice as settled.
        /// No real payment processing is performed (per Assignment 3 spec).
        /// </summary>
        public void ProcessPaymentStub(Order order, Invoice invoice)
        {
            order.UpdateStatus(OrderStatus.Paid);
            invoice.MarkAsSettled();
        }

        // ── Private helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Checks that every cart item has sufficient current stock.
        /// Returns false and prints the problem if any item fails.
        /// </summary>
        private bool ValidateStock(ShoppingCart cart)
        {
            foreach (CartItem item in cart.Items)
            {
                Book? current = _catalogue.FindByISBN(item.Book.ISBN);
                if (current == null || !current.IsAvailable ||
                    current.StockQuantity < item.Quantity)
                    return false;
            }
            return true;
        }

        /// <summary>Deducts purchased quantities from each book's stock.</summary>
        private void DeductStock(ShoppingCart cart)
        {
            foreach (CartItem item in cart.Items)
            {
                Book? book = _catalogue.FindByISBN(item.Book.ISBN);
                book?.UpdateStock(book.StockQuantity - item.Quantity);
            }
        }
    }
}
