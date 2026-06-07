// Book.cs
// Represents a single book available for sale in the Online Bookstore System.
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

namespace BookstoreApp.Models
{
    /// <summary>
    /// Represents a book available for sale in the catalogue.
    /// Uniquely identified by ISBN. Price is locked per OrderLine at purchase time.
    /// </summary>
    public class Book
    {
        // ── Properties ────────────────────────────────────────────────────────

        public string ISBN { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Genre { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsAvailable { get; private set; }

        // ── Constructor ───────────────────────────────────────────────────────

        public Book(string isbn, string title, string author, string genre,
                    string description, decimal price, int stockQuantity)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("ISBN cannot be empty.", nameof(isbn));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Author cannot be empty.", nameof(author));
            if (price < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(price));
            if (stockQuantity < 0)
                throw new ArgumentException("Stock cannot be negative.", nameof(stockQuantity));

            ISBN = isbn;
            Title = title;
            Author = author;
            Genre = genre;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            IsAvailable = stockQuantity > 0;
        }

        // ── Methods ───────────────────────────────────────────────────────────

        /// <summary>Updates stock and recalculates availability.</summary>
        public void UpdateStock(int newQuantity)
        {
            if (newQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative.");
            StockQuantity = newQuantity;
            IsAvailable = newQuantity > 0;
        }

        /// <summary>Updates editable details. ISBN is immutable after creation.</summary>
        public void UpdateDetails(string title, string author, string genre,
                                  string description, decimal price)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Author cannot be empty.");
            if (price < 0)
                throw new ArgumentException("Price cannot be negative.");

            Title = title;
            Author = author;
            Genre = genre;
            Description = description;
            Price = price;
        }

        /// <summary>
        /// Marks the book as unavailable without removing it from the catalogue.
        /// Used when a book has active orders and cannot be deleted.
        /// </summary>
        public void Disable()
        {
            IsAvailable = false;
        }

        public override string ToString()
        {
            string stock = IsAvailable ? $"In Stock ({StockQuantity})" : "Unavailable";
            return $"[{ISBN}] {Title} by {Author} | {Genre} | ${Price:F2} | {stock}";
        }

        public string ToDetailString()
        {
            return $"Title       : {Title}\n" +
                   $"Author      : {Author}\n" +
                   $"ISBN        : {ISBN}\n" +
                   $"Genre       : {Genre}\n" +
                   $"Price       : ${Price:F2}\n" +
                   $"Stock       : {StockQuantity}\n" +
                   $"Available   : {(IsAvailable ? "Yes" : "No")}\n" +
                   $"Description : {Description}";
        }
    }
}
