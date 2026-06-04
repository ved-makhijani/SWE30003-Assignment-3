// BookTests.cs
// Unit tests for the Book class.
// Framework: MSTest
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using BookstoreApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookstoreApp.Tests
{
    [TestClass]
    public class BookTests
    {
        // ── Helper ────────────────────────────────────────────────────────────

        private static Book MakeBook(int stock = 5, decimal price = 19.99m) =>
            new("978-1-1234-5678-9", "Test Book", "Test Author",
                "Fiction", "A test description.", price, stock);

        // ── Constructor validation ─────────────────────────────────────────────

        [TestMethod]
        public void Constructor_ValidInputs_CreatesBook()
        {
            Book book = MakeBook(stock: 10);
            Assert.AreEqual("978-1-1234-5678-9", book.ISBN);
            Assert.AreEqual("Test Book", book.Title);
            Assert.AreEqual("Test Author", book.Author);
            Assert.AreEqual(10, book.StockQuantity);
            Assert.IsTrue(book.IsAvailable);
        }

        [TestMethod]
        public void Constructor_ZeroStock_IsNotAvailable()
        {
            Book book = MakeBook(stock: 0);
            Assert.AreEqual(0, book.StockQuantity);
            Assert.IsFalse(book.IsAvailable);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyISBN_ThrowsArgumentException()
        {
            new Book("", "Title", "Author", "Genre", "Desc", 10m, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyTitle_ThrowsArgumentException()
        {
            new Book("978-1-0000-0000-0", "", "Author", "Genre", "Desc", 10m, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NegativePrice_ThrowsArgumentException()
        {
            new Book("978-1-0000-0000-0", "Title", "Author", "Genre", "Desc", -1m, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NegativeStock_ThrowsArgumentException()
        {
            new Book("978-1-0000-0000-0", "Title", "Author", "Genre", "Desc", 10m, -1);
        }

        // ── UpdateStock ───────────────────────────────────────────────────────

        [TestMethod]
        public void UpdateStock_PositiveValue_UpdatesStockAndAvailability()
        {
            Book book = MakeBook(stock: 5);
            book.UpdateStock(20);
            Assert.AreEqual(20, book.StockQuantity);
            Assert.IsTrue(book.IsAvailable);
        }

        [TestMethod]
        public void UpdateStock_Zero_SetsUnavailable()
        {
            Book book = MakeBook(stock: 5);
            book.UpdateStock(0);
            Assert.AreEqual(0, book.StockQuantity);
            Assert.IsFalse(book.IsAvailable);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateStock_NegativeValue_ThrowsArgumentException()
        {
            Book book = MakeBook();
            book.UpdateStock(-1);
        }

        // ── UpdateDetails ─────────────────────────────────────────────────────

        [TestMethod]
        public void UpdateDetails_ValidInputs_UpdatesFields()
        {
            Book book = MakeBook();
            book.UpdateDetails("New Title", "New Author", "Non-Fiction", "New Desc", 29.99m);
            Assert.AreEqual("New Title", book.Title);
            Assert.AreEqual("New Author", book.Author);
            Assert.AreEqual(29.99m, book.Price);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateDetails_EmptyTitle_ThrowsArgumentException()
        {
            Book book = MakeBook();
            book.UpdateDetails("", "Author", "Genre", "Desc", 10m);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateDetails_NegativePrice_ThrowsArgumentException()
        {
            Book book = MakeBook();
            book.UpdateDetails("Title", "Author", "Genre", "Desc", -5m);
        }

        // ── Disable ───────────────────────────────────────────────────────────

        [TestMethod]
        public void Disable_AvailableBook_SetsUnavailable()
        {
            Book book = MakeBook(stock: 10);
            Assert.IsTrue(book.IsAvailable);
            book.Disable();
            Assert.IsFalse(book.IsAvailable);
        }

        [TestMethod]
        public void Disable_DoesNotChangeStockQuantity()
        {
            Book book = MakeBook(stock: 10);
            book.Disable();
            Assert.AreEqual(10, book.StockQuantity); // stock still 10, just hidden
        }
    }
}
