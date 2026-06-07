// CartItem.cs
// Data-holder class: a specific book and quantity in a ShoppingCart.
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

namespace BookstoreApp.Models
{
    /// <summary>
    /// Data-holder representing a book and its selected quantity
    /// within a customer's ShoppingCart. Temporary — discarded after checkout.
    /// </summary>
    public class CartItem
    {
        public Book Book { get; private set; }
        public int Quantity { get; private set; }
        public decimal LineTotal => Book.Price * Quantity;

        public CartItem(Book book, int quantity)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1.", nameof(quantity));

            Book = book;
            Quantity = quantity;
        }

        /// <summary>Updates the quantity for this cart item.</summary>
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be at least 1.");
            if (newQuantity > Book.StockQuantity)
                throw new ArgumentException(
                    $"Only {Book.StockQuantity} copies of '{Book.Title}' available.");
            Quantity = newQuantity;
        }

        public override string ToString() =>
            $"  {Book.Title} (x{Quantity}) @ ${Book.Price:F2} = ${LineTotal:F2}";
    }
}
