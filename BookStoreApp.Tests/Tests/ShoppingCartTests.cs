// ShoppingCartTests.cs
// Unit tests for the ShoppingCart class.
// Framework: MSTest
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using BookstoreApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookstoreApp.Tests
{
    [TestClass]
    public class ShoppingCartTests
    {
        private ShoppingCart _cart = null!;
        private Book _bookA = null!;
        private Book _bookB = null!;

        [TestInitialize]
        public void Setup()
        {
            _cart = new ShoppingCart();
            _bookA = new Book("978-0-0001-0001-1", "Book A", "Author A",
                              "Fiction", "Desc A", 10.00m, 5);
            _bookB = new Book("978-0-0001-0001-2", "Book B", "Author B",
                              "Fiction", "Desc B", 20.00m, 3);
        }

        // ── AddItem ───────────────────────────────────────────────────────────

        [TestMethod]
        public void AddItem_AvailableBook_ReturnsTrue()
        {
            bool result = _cart.AddItem(_bookA);
            Assert.IsTrue(result);
            Assert.AreEqual(1, _cart.Items.Count);
        }

        [TestMethod]
        public void AddItem_UnavailableBook_ReturnsFalse()
        {
            Book outOfStock = new Book("978-0-0099-0000-1", "OOS Book", "Author",
                                       "Genre", "Desc", 10m, 0);
            bool result = _cart.AddItem(outOfStock);
            Assert.IsFalse(result);
            Assert.AreEqual(0, _cart.Items.Count);
        }

        [TestMethod]
        public void AddItem_ExceedsStock_ReturnsFalse()
        {
            bool result = _cart.AddItem(_bookA, 99); // only 5 in stock
            Assert.IsFalse(result);
            Assert.AreEqual(0, _cart.Items.Count);
        }

        [TestMethod]
        public void AddItem_SameBookTwice_MergesQuantity()
        {
            _cart.AddItem(_bookA, 2);
            _cart.AddItem(_bookA, 2);
            Assert.AreEqual(1, _cart.Items.Count);        // still one entry
            Assert.AreEqual(4, _cart.Items[0].Quantity);  // merged to 4
        }

        [TestMethod]
        public void AddItem_MergedQuantityExceedsStock_ReturnsFalse()
        {
            _cart.AddItem(_bookA, 4); // 4 added, stock = 5
            bool result = _cart.AddItem(_bookA, 2); // 4+2=6, exceeds stock of 5
            Assert.IsFalse(result);
            Assert.AreEqual(4, _cart.Items[0].Quantity); // unchanged
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddItem_ZeroQuantity_ThrowsArgumentException()
        {
            _cart.AddItem(_bookA, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddItem_NullBook_ThrowsArgumentNullException()
        {
            _cart.AddItem(null!);
        }

        // ── RemoveItem ────────────────────────────────────────────────────────

        [TestMethod]
        public void RemoveItem_ExistingISBN_RemovesAndReturnsTrue()
        {
            _cart.AddItem(_bookA);
            bool result = _cart.RemoveItem(_bookA.ISBN);
            Assert.IsTrue(result);
            Assert.AreEqual(0, _cart.Items.Count);
        }

        [TestMethod]
        public void RemoveItem_NonExistentISBN_ReturnsFalse()
        {
            bool result = _cart.RemoveItem("999-9-9999-9999-9");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RemoveItem_LastItem_CartBecomesEmpty()
        {
            _cart.AddItem(_bookA);
            _cart.RemoveItem(_bookA.ISBN);
            Assert.IsTrue(_cart.IsEmpty);
        }

        // ── UpdateQuantity ────────────────────────────────────────────────────

        [TestMethod]
        public void UpdateQuantity_ValidNewQuantity_UpdatesSuccessfully()
        {
            _cart.AddItem(_bookA, 1);
            bool result = _cart.UpdateQuantity(_bookA.ISBN, 3);
            Assert.IsTrue(result);
            Assert.AreEqual(3, _cart.Items[0].Quantity);
        }

        [TestMethod]
        public void UpdateQuantity_ExceedsStock_ReturnsFalse()
        {
            _cart.AddItem(_bookA, 1);
            bool result = _cart.UpdateQuantity(_bookA.ISBN, 99);
            Assert.IsFalse(result);
            Assert.AreEqual(1, _cart.Items[0].Quantity); // unchanged
        }

        [TestMethod]
        public void UpdateQuantity_ZeroOrNegative_ReturnsFalse()
        {
            _cart.AddItem(_bookA, 2);
            bool result = _cart.UpdateQuantity(_bookA.ISBN, 0);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UpdateQuantity_NonExistentISBN_ReturnsFalse()
        {
            bool result = _cart.UpdateQuantity("999-0-0000-0000-0", 1);
            Assert.IsFalse(result);
        }

        // ── CalculateTotal ────────────────────────────────────────────────────

        [TestMethod]
        public void CalculateTotal_MultipleItems_ReturnsSumOfLineTotals()
        {
            _cart.AddItem(_bookA, 2); // 2 × $10.00 = $20.00
            _cart.AddItem(_bookB, 1); // 1 × $20.00 = $20.00
            Assert.AreEqual(40.00m, _cart.CalculateTotal());
        }

        [TestMethod]
        public void CalculateTotal_EmptyCart_ReturnsZero()
        {
            Assert.AreEqual(0m, _cart.CalculateTotal());
        }

        // ── Clear ─────────────────────────────────────────────────────────────

        [TestMethod]
        public void Clear_WithItems_EmptiesCart()
        {
            _cart.AddItem(_bookA);
            _cart.AddItem(_bookB);
            _cart.Clear();
            Assert.IsTrue(_cart.IsEmpty);
            Assert.AreEqual(0, _cart.Items.Count);
        }

        // ── IsEmpty ───────────────────────────────────────────────────────────

        [TestMethod]
        public void IsEmpty_NewCart_ReturnsTrue()
        {
            Assert.IsTrue(_cart.IsEmpty);
        }

        [TestMethod]
        public void IsEmpty_AfterAddItem_ReturnsFalse()
        {
            _cart.AddItem(_bookA);
            Assert.IsFalse(_cart.IsEmpty);
        }
    }
}
