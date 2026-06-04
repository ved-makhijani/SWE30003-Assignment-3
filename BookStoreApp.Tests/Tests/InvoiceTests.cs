// InvoiceTests.cs
// Unit tests for the Invoice class.
// Framework: MSTest
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using BookstoreApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookstoreApp.Tests
{
    [TestClass]
    public class InvoiceTests
    {
        private Order _order = null!;

        [TestInitialize]
        public void Setup()
        {
            Order.ResetIdCounter(1000);
            Invoice.ResetIdCounter(5000);

            Book book = new Book("978-0-0005-0001-1", "Invoice Book", "Author",
                                 "Genre", "Desc", 25.00m, 10);
            List<CartItem> items = new List<CartItem> { new CartItem(book, 2) };
            _order = new Order("invoice@test.com", "456 Test Ave", items);
        }

        [TestMethod]
        public void Constructor_ValidOrder_CreatesInvoice()
        {
            Invoice invoice = new Invoice(_order);
            Assert.AreEqual(_order, invoice.Order);
            Assert.IsFalse(invoice.IsSettled);
        }

        [TestMethod]
        public void Constructor_AmountDue_MatchesOrderTotal()
        {
            Invoice invoice = new Invoice(_order);
            Assert.AreEqual(_order.CalculateTotal(), invoice.AmountDue);
            Assert.AreEqual(50.00m, invoice.AmountDue); // 2 × $25
        }

        [TestMethod]
        public void Constructor_AssignsUniqueInvoiceIds()
        {
            Book book = new Book("978-0-0005-0001-2", "Book 2", "Author",
                                 "Genre", "Desc", 10m, 5);
            var items = new List<CartItem> { new CartItem(book, 1) };
            Order order2 = new Order("b@test.com", "789 St", items);

            Invoice inv1 = new Invoice(_order);
            Invoice inv2 = new Invoice(order2);
            Assert.AreNotEqual(inv1.InvoiceId, inv2.InvoiceId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullOrder_ThrowsArgumentNullException()
        {
            new Invoice(null!);
        }

        [TestMethod]
        public void MarkAsSettled_SetsIsSettledToTrue()
        {
            Invoice invoice = new Invoice(_order);
            Assert.IsFalse(invoice.IsSettled);
            invoice.MarkAsSettled();
            Assert.IsTrue(invoice.IsSettled);
        }

        [TestMethod]
        public void MarkAsSettled_CalledTwice_RemainsSettled()
        {
            Invoice invoice = new Invoice(_order);
            invoice.MarkAsSettled();
            invoice.MarkAsSettled();
            Assert.IsTrue(invoice.IsSettled);
        }
    }
}
