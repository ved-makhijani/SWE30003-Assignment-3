// OrderTests.cs
// Unit tests for Order and OrderLine classes.
// Framework: MSTest
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using BookstoreApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookstoreApp.Tests
{
    [TestClass]
    public class OrderTests
    {
        private Book _bookA = null!;
        private Book _bookB = null!;
        private List<CartItem> _cartItems = null!;

        [TestInitialize]
        public void Setup()
        {
            Order.ResetIdCounter(1000);
            _bookA = new Book("978-0-0002-0001-1", "Order Book A", "Author A",
                              "Fiction", "Desc", 15.00m, 10);
            _bookB = new Book("978-0-0002-0001-2", "Order Book B", "Author B",
                              "Fiction", "Desc", 25.00m, 10);
            _cartItems = new List<CartItem>
            {
                new CartItem(_bookA, 2),
                new CartItem(_bookB, 1)
            };
        }

        // ── Constructor ───────────────────────────────────────────────────────

        [TestMethod]
        public void Constructor_ValidInputs_CreatesOrder()
        {
            Order order = new Order("test@test.com", "123 Main St", _cartItems);
            Assert.AreEqual("test@test.com", order.CustomerEmail);
            Assert.AreEqual("123 Main St", order.DeliveryAddress);
            Assert.AreEqual(OrderStatus.PendingPayment, order.Status);
            Assert.AreEqual(2, order.Lines.Count);
        }

        [TestMethod]
        public void Constructor_AssignsUniqueOrderIds()
        {
            Order order1 = new Order("a@test.com", "Addr 1", _cartItems);
            Order order2 = new Order("b@test.com", "Addr 2", _cartItems);
            Assert.AreNotEqual(order1.OrderId, order2.OrderId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyEmail_ThrowsArgumentException()
        {
            new Order("", "123 St", _cartItems);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyAddress_ThrowsArgumentException()
        {
            new Order("test@test.com", "", _cartItems);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyCartItems_ThrowsArgumentException()
        {
            new Order("test@test.com", "123 St", new List<CartItem>());
        }

        // ── CalculateTotal ────────────────────────────────────────────────────

        [TestMethod]
        public void CalculateTotal_SumsAllOrderLineLineTotals()
        {
            // BookA: 2 × $15.00 = $30.00
            // BookB: 1 × $25.00 = $25.00
            // Total = $55.00
            Order order = new Order("test@test.com", "123 Main St", _cartItems);
            Assert.AreEqual(55.00m, order.CalculateTotal());
        }

        // ── UpdateStatus ──────────────────────────────────────────────────────

        [TestMethod]
        public void UpdateStatus_ValidTransition_ChangesStatus()
        {
            Order order = new Order("test@test.com", "123 Main St", _cartItems);
            order.UpdateStatus(OrderStatus.Paid);
            Assert.AreEqual(OrderStatus.Paid, order.Status);
        }

        [TestMethod]
        public void UpdateStatus_ThroughFullLifecycle_ChangesCorrectly()
        {
            Order order = new Order("test@test.com", "123 Main St", _cartItems);
            order.UpdateStatus(OrderStatus.Paid);
            order.UpdateStatus(OrderStatus.Packaged);
            order.UpdateStatus(OrderStatus.Dispatched);
            order.UpdateStatus(OrderStatus.Delivered);
            Assert.AreEqual(OrderStatus.Delivered, order.Status);
        }

        // ── OrderLine locking ─────────────────────────────────────────────────

        [TestMethod]
        public void OrderLine_LocksPriceAtCreation_IndependentOfBookPriceChange()
        {
            Order order = new Order("test@test.com", "123 Main St", _cartItems);
            decimal originalPrice = order.Lines[0].UnitPriceAtPurchase;

            // Change the book price after order creation
            _bookA.UpdateDetails(_bookA.Title, _bookA.Author, _bookA.Genre,
                                  _bookA.Description, 999.00m);

            // Order line price must remain at the original price
            Assert.AreEqual(originalPrice, order.Lines[0].UnitPriceAtPurchase);
            Assert.AreEqual(15.00m, order.Lines[0].UnitPriceAtPurchase);
        }

        [TestMethod]
        public void OrderLine_LineTotal_IsQuantityTimesLockedPrice()
        {
            Order order = new Order("test@test.com", "123 Main St", _cartItems);
            // BookA: 2 × $15.00 = $30.00
            Assert.AreEqual(30.00m, order.Lines[0].LineTotal);
        }
    }

    // ── OrderLine standalone tests ────────────────────────────────────────────

    [TestClass]
    public class OrderLineTests
    {
        [TestMethod]
        public void Constructor_ValidInputs_CreatesOrderLine()
        {
            Book book = new Book("978-0-0003-0001-1", "Test", "Author",
                                 "Genre", "Desc", 12.50m, 5);
            OrderLine line = new OrderLine(book, 3);
            Assert.AreEqual(3, line.Quantity);
            Assert.AreEqual(12.50m, line.UnitPriceAtPurchase);
            Assert.AreEqual(37.50m, line.LineTotal);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_ZeroQuantity_ThrowsArgumentException()
        {
            Book book = new Book("978-0-0003-0001-2", "Test", "Author",
                                 "Genre", "Desc", 10m, 5);
            new OrderLine(book, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullBook_ThrowsArgumentNullException()
        {
            new OrderLine(null!, 1);
        }
    }
}
