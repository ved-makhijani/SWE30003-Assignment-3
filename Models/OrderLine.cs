// OrderLine.cs
// Data-holder: a purchased book and quantity within a confirmed Order.
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

namespace BookstoreApp.Models
{
    /// <summary>
    /// Data-holder representing a specific book and quantity within a confirmed Order.
    /// Unit price is locked at the time of order creation so that later
    /// price changes to the Book do not affect historical order totals.
    /// </summary>
    public class OrderLine
    {
        public Book Book { get; private set; }
        public int Quantity { get; private set; }

        /// <summary>Price locked at time of purchase — immutable after creation.</summary>
        public decimal UnitPriceAtPurchase { get; private set; }

        public decimal LineTotal => UnitPriceAtPurchase * Quantity;

        public OrderLine(Book book, int quantity)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1.");

            Book = book;
            Quantity = quantity;
            UnitPriceAtPurchase = book.Price;
        }

        public override string ToString() =>
            $"  {Book.Title} (x{Quantity}) @ ${UnitPriceAtPurchase:F2} = ${LineTotal:F2}";
    }
}
