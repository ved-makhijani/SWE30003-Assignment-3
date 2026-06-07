// Catalogue.cs
// Manages the full collection of books available in the Online Bookstore.
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

namespace BookstoreApp.Models
{
    /// <summary>
    /// Represents the complete collection of books available for sale.
    /// Managed by StoreManager/Administrator. Loaded from Database on startup.
    /// </summary>
    public class Catalogue
    {
        private readonly List<Book> _books = new();

        // ── Catalogue Management ──────────────────────────────────────────────

        /// <summary>
        /// Adds a book. Rejects duplicate ISBNs.
        /// Returns true on success, false if ISBN already exists.
        /// </summary>
        public bool AddBook(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (ISBNExists(book.ISBN)) return false;
            _books.Add(book);
            return true;
        }

        /// <summary>
        /// Removes a book by ISBN. Returns false if not found.
        /// Caller must verify no active orders reference this book first.
        /// </summary>
        public bool RemoveBook(string isbn)
        {
            Book? book = FindByISBN(isbn);
            if (book == null) return false;
            _books.Remove(book);
            return true;
        }

        /// <summary>
        /// Marks a book unavailable instead of deleting it.
        /// Used when the book is referenced by active orders.
        /// </summary>
        public bool DisableBook(string isbn)
        {
            Book? book = FindByISBN(isbn);
            if (book == null) return false;
            book.Disable();
            return true;
        }

        // ── Search and Filter ─────────────────────────────────────────────────

        /// <summary>Returns all available books (customer-facing view).</summary>
        public List<Book> ListAvailableBooks() =>
            _books.Where(b => b.IsAvailable).ToList();

        /// <summary>Returns all books including unavailable (admin view).</summary>
        public List<Book> ListAllBooks() => new List<Book>(_books);

        /// <summary>
        /// Searches available books by keyword across title, author, and genre.
        /// Case-insensitive.
        /// </summary>
        public List<Book> SearchBooks(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return ListAvailableBooks();
            string kw = keyword.ToLower();
            return _books.Where(b =>
                b.IsAvailable &&
                (b.Title.ToLower().Contains(kw) ||
                 b.Author.ToLower().Contains(kw) ||
                 b.Genre.ToLower().Contains(kw))
            ).ToList();
        }

        /// <summary>Filters available books by genre (case-insensitive).</summary>
        public List<Book> FilterByGenre(string genre) =>
            _books.Where(b =>
                b.IsAvailable && b.Genre.ToLower() == genre.ToLower()
            ).ToList();

        /// <summary>Filters available books at or below the given max price.</summary>
        public List<Book> FilterByMaxPrice(decimal maxPrice) =>
            _books.Where(b => b.IsAvailable && b.Price <= maxPrice).ToList();

        /// <summary>Finds a book by exact ISBN. Returns null if not found.</summary>
        public Book? FindByISBN(string isbn) =>
            _books.FirstOrDefault(b => b.ISBN == isbn);

        /// <summary>Returns true if an ISBN already exists in the catalogue.</summary>
        public bool ISBNExists(string isbn) =>
            _books.Any(b => b.ISBN == isbn);

        public int Count => _books.Count;

        public List<Book> FilterByAuthor(string author) =>
            _books.Where(b => b.IsAvailable && b.Author.ToLower().Contains(author.ToLower())).ToList();
    }

    
}
