// CatalogueTests.cs
// Unit tests for the Catalogue class.
// Framework: MSTest
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using BookstoreApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookstoreApp.Tests
{
    [TestClass]
    public class CatalogueTests
    {
        private Catalogue _catalogue = null!;
        private Book _book1 = null!;
        private Book _book2 = null!;

        [TestInitialize]
        public void Setup()
        {
            _catalogue = new Catalogue();
            _book1 = new Book("978-0-0001-0000-1", "Clean Code", "Robert Martin",
                              "Technology", "Writing maintainable code.", 39.99m, 5);
            _book2 = new Book("978-0-0001-0000-2", "Refactoring", "Martin Fowler",
                              "Technology", "Improving existing code.", 44.99m, 3);
        }

        // ── AddBook ───────────────────────────────────────────────────────────

        [TestMethod]
        public void AddBook_NewISBN_ReturnsTrue()
        {
            bool result = _catalogue.AddBook(_book1);
            Assert.IsTrue(result);
            Assert.AreEqual(1, _catalogue.Count);
        }

        [TestMethod]
        public void AddBook_DuplicateISBN_ReturnsFalse()
        {
            _catalogue.AddBook(_book1);
            bool result = _catalogue.AddBook(_book1);
            Assert.IsFalse(result);
            Assert.AreEqual(1, _catalogue.Count); // still only 1
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddBook_NullBook_ThrowsArgumentNullException()
        {
            _catalogue.AddBook(null!);
        }

        // ── RemoveBook ────────────────────────────────────────────────────────

        [TestMethod]
        public void RemoveBook_ExistingISBN_RemovesAndReturnsTrue()
        {
            _catalogue.AddBook(_book1);
            bool result = _catalogue.RemoveBook(_book1.ISBN);
            Assert.IsTrue(result);
            Assert.AreEqual(0, _catalogue.Count);
        }

        [TestMethod]
        public void RemoveBook_NonExistentISBN_ReturnsFalse()
        {
            bool result = _catalogue.RemoveBook("999-9-9999-9999-9");
            Assert.IsFalse(result);
        }

        // ── DisableBook ───────────────────────────────────────────────────────

        [TestMethod]
        public void DisableBook_ExistingISBN_MarksUnavailable()
        {
            _catalogue.AddBook(_book1);
            bool result = _catalogue.DisableBook(_book1.ISBN);
            Assert.IsTrue(result);
            Assert.IsFalse(_book1.IsAvailable);
        }

        [TestMethod]
        public void DisableBook_DisabledBookNotInAvailableList()
        {
            _catalogue.AddBook(_book1);
            _catalogue.DisableBook(_book1.ISBN);
            List<Book> available = _catalogue.ListAvailableBooks();
            Assert.AreEqual(0, available.Count);
        }

        [TestMethod]
        public void DisableBook_DisabledBookStillInAllBooksList()
        {
            _catalogue.AddBook(_book1);
            _catalogue.DisableBook(_book1.ISBN);
            List<Book> all = _catalogue.ListAllBooks();
            Assert.AreEqual(1, all.Count); // still in full list
        }

        // ── SearchBooks ───────────────────────────────────────────────────────

        [TestMethod]
        public void SearchBooks_MatchingTitle_ReturnsBook()
        {
            _catalogue.AddBook(_book1);
            _catalogue.AddBook(_book2);
            List<Book> results = _catalogue.SearchBooks("Clean");
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Clean Code", results[0].Title);
        }

        [TestMethod]
        public void SearchBooks_MatchingAuthor_ReturnsBook()
        {
            _catalogue.AddBook(_book1);
            _catalogue.AddBook(_book2);
            List<Book> results = _catalogue.SearchBooks("Fowler");
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Refactoring", results[0].Title);
        }

        [TestMethod]
        public void SearchBooks_CaseInsensitive_ReturnsBook()
        {
            _catalogue.AddBook(_book1);
            List<Book> results = _catalogue.SearchBooks("clean code");
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void SearchBooks_NoMatch_ReturnsEmptyList()
        {
            _catalogue.AddBook(_book1);
            List<Book> results = _catalogue.SearchBooks("zzznomatch");
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void SearchBooks_UnavailableBook_NotReturned()
        {
            _catalogue.AddBook(_book1);
            _catalogue.DisableBook(_book1.ISBN);
            List<Book> results = _catalogue.SearchBooks("Clean");
            Assert.AreEqual(0, results.Count);
        }

        // ── FilterByGenre ─────────────────────────────────────────────────────

        [TestMethod]
        public void FilterByGenre_MatchingGenre_ReturnsBooks()
        {
            _catalogue.AddBook(_book1);
            _catalogue.AddBook(_book2);
            List<Book> results = _catalogue.FilterByGenre("Technology");
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void FilterByGenre_CaseInsensitive_ReturnsBooks()
        {
            _catalogue.AddBook(_book1);
            List<Book> results = _catalogue.FilterByGenre("technology");
            Assert.AreEqual(1, results.Count);
        }

        // ── FilterByMaxPrice ──────────────────────────────────────────────────

        [TestMethod]
        public void FilterByMaxPrice_ReturnsOnlyBooksAtOrBelow()
        {
            _catalogue.AddBook(_book1); // $39.99
            _catalogue.AddBook(_book2); // $44.99
            List<Book> results = _catalogue.FilterByMaxPrice(40.00m);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Clean Code", results[0].Title);
        }

        // ── FindByISBN ────────────────────────────────────────────────────────

        [TestMethod]
        public void FindByISBN_ExistingISBN_ReturnsBook()
        {
            _catalogue.AddBook(_book1);
            Book? found = _catalogue.FindByISBN(_book1.ISBN);
            Assert.IsNotNull(found);
            Assert.AreEqual(_book1.Title, found.Title);
        }

        [TestMethod]
        public void FindByISBN_NonExistentISBN_ReturnsNull()
        {
            Book? found = _catalogue.FindByISBN("999-9-9999-9999-9");
            Assert.IsNull(found);
        }

        // ── ISBNExists ────────────────────────────────────────────────────────

        [TestMethod]
        public void ISBNExists_ExistingISBN_ReturnsTrue()
        {
            _catalogue.AddBook(_book1);
            Assert.IsTrue(_catalogue.ISBNExists(_book1.ISBN));
        }

        [TestMethod]
        public void ISBNExists_NonExistentISBN_ReturnsFalse()
        {
            Assert.IsFalse(_catalogue.ISBNExists("000-0-0000-0000-0"));
        }
    }
}
