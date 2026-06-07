// ShoppingCart.cs
// Manages the customer's temporary book selection before checkout.
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

namespace BookstoreApp.Models
{
    /// <summary>
    /// Temporary container holding books selected by a customer before checkout.
    /// A new ShoppingCart is created per login session.
    /// </summary>
    public class ShoppingCart
    {
        private readonly List<CartItem> _items = new();

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
        public bool IsEmpty => _items.Count == 0;

        // ── Operations ────────────────────────────────────────────────────────

        /// <summary>
        /// Adds a book to the cart. If the book already exists in the cart,
        /// increments the existing quantity rather than adding a duplicate.
        /// Returns false if the book is unavailable or stock is insufficient.
        /// </summary>
        public bool AddItem(Book book, int quantity = 1)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1.");

            if (!book.IsAvailable)
                return false;

            CartItem? existing = _items.FirstOrDefault(i => i.Book.ISBN == book.ISBN);
            int totalRequested = (existing?.Quantity ?? 0) + quantity;

            if (totalRequested > book.StockQuantity)
                return false;

            if (existing != null)
                existing.UpdateQuantity(totalRequested);
            else
                _items.Add(new CartItem(book, quantity));

            return true;
        }

        /// <summary>
        /// Removes a book from the cart by ISBN.
        /// Returns false if the ISBN is not in the cart.
        /// </summary>
        public bool RemoveItem(string isbn)
        {
            CartItem? item = _items.FirstOrDefault(i => i.Book.ISBN == isbn);
            if (item == null) return false;
            _items.Remove(item);
            return true;
        }

        /// <summary>
        /// Updates the quantity of a specific cart item.
        /// Returns false if the ISBN is not in the cart or quantity is invalid.
        /// </summary>
        public bool UpdateQuantity(string isbn, int newQuantity)
        {
            if (newQuantity <= 0) return false;

            CartItem? item = _items.FirstOrDefault(i => i.Book.ISBN == isbn);
            if (item == null) return false;

            try
            {
                item.UpdateQuantity(newQuantity);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>Removes all items from the cart.</summary>
        public void Clear() => _items.Clear();

        /// <summary>Returns the sum of all item line totals.</summary>
        public decimal CalculateTotal() => _items.Sum(i => i.LineTotal);

        /// <summary>Prints the cart contents to the console.</summary>
        public void DisplayContents()
        {
            if (IsEmpty)
            {
                Console.WriteLine("  Your cart is empty.");
                return;
            }

            Console.WriteLine("  ┌─────────────────────────────────────────────┐");
            Console.WriteLine("  │                SHOPPING CART                │");
            Console.WriteLine("  ├─────────────────────────────────────────────┤");
            foreach (CartItem item in _items)
                Console.WriteLine(item.ToString());
            Console.WriteLine("  ├─────────────────────────────────────────────┤");
            Console.WriteLine($"  │ TOTAL: ${CalculateTotal(),-36:F2}│");
            Console.WriteLine("  └─────────────────────────────────────────────┘");
        }
    }
}
