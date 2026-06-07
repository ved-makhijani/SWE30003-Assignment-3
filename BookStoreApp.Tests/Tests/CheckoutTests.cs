// CheckoutTests.cs
// Unit tests for the Checkout class — the most complex class in Ved's scope.
// Framework: MSTest
// Coding standard: Microsoft C# Coding Conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

using BookstoreApp.Data;
using BookstoreApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookstoreApp.Tests
{
    [TestClass]
    public class CheckoutTests
    {
        private Catalogue _catalogue = null!;
        private Database _db = null!;
        private Checkout _checkout = null!;
        private ShoppingCart _cart = null!;
        private Book _bookA = null!;
        private Book _bookB = null!;

        [TestInitialize]
        public void Setup()
        {
            Database.Reset();
            Order.ResetIdCounter(1000);
            Invoice.ResetIdCounter(5000);

            _db = Database.GetInstance();
            _catalogue = new Catalogue();

            _bookA = new Book("978-0-0004-0001-1", "Checkout Book A", "Author A",
                              "Fiction", "Desc A", 10.00m, 5);
            _bookB = new Book("978-0-0004-0001-2", "Checkout Book B", "Author B",
                              "Fiction", "Desc B", 20.00m, 3);

            _catalogue.AddBook(_bookA);
            _catalogue.AddBook(_bookB);

            _checkout = new Checkout(_db, _catalogue);
            _cart = new ShoppingCart();
        }

        // ── ProcessCheckout — success ─────────────────────────────────────────

        [TestMethod]
        public void ProcessCheckout_ValidCart_ReturnsInvoice()
        {
            _cart.AddItem(_bookA, 2);
            Invoice? invoice = _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            Assert.IsNotNull(invoice);
        }

        [TestMethod]
        public void ProcessCheckout_ValidCart_InvoiceAmountMatchesCartTotal()
        {
            _cart.AddItem(_bookA, 2); // 2 × $10 = $20
            _cart.AddItem(_bookB, 1); // 1 × $20 = $20
            Invoice? invoice = _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            Assert.IsNotNull(invoice);
            Assert.AreEqual(40.00m, invoice!.AmountDue);
        }

        [TestMethod]
        public void ProcessCheckout_Success_ClearsCart()
        {
            _cart.AddItem(_bookA, 1);
            _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            Assert.IsTrue(_cart.IsEmpty);
        }

        [TestMethod]
        public void ProcessCheckout_Success_DeductsBookStock()
        {
            int originalStock = _bookA.StockQuantity; // 5
            _cart.AddItem(_bookA, 3);
            _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            Assert.AreEqual(originalStock - 3, _bookA.StockQuantity); // now 2
        }

        [TestMethod]
        public void ProcessCheckout_Success_OrderSavedToDatabase()
        {
            _cart.AddItem(_bookA, 1);
            _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            var orders = _db.GetOrdersByCustomer("test@test.com");
            Assert.AreEqual(1, orders.Count);
        }

        [TestMethod]
        public void ProcessCheckout_Success_InvoiceSavedToDatabase()
        {
            _cart.AddItem(_bookA, 1);
            _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            var invoices = _db.GetAllInvoices();
            Assert.AreEqual(1, invoices.Count);
        }

        [TestMethod]
        public void ProcessCheckout_Success_OrderStatusIsPendingPayment()
        {
            _cart.AddItem(_bookA, 1);
            Invoice? invoice = _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            Assert.AreEqual(OrderStatus.PendingPayment, invoice!.Order.Status);
        }

        // ── ProcessCheckout — failure cases ───────────────────────────────────

        [TestMethod]
        public void ProcessCheckout_EmptyCart_ReturnsNull()
        {
            Invoice? invoice = _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            Assert.IsNull(invoice);
        }

        [TestMethod]
        public void ProcessCheckout_EmptyAddress_ReturnsNull()
        {
            _cart.AddItem(_bookA, 1);
            Invoice? invoice = _checkout.ProcessCheckout(_cart, "test@test.com", "");
            Assert.IsNull(invoice);
        }

        [TestMethod]
        public void ProcessCheckout_WhitespaceAddress_ReturnsNull()
        {
            _cart.AddItem(_bookA, 1);
            Invoice? invoice = _checkout.ProcessCheckout(_cart, "test@test.com", "   ");
            Assert.IsNull(invoice);
        }

        [TestMethod]
        public void ProcessCheckout_InsufficientStock_ReturnsNull()
        {
            // Cart has 4, but stock is only 3 by the time we check
            _bookA.UpdateStock(2); // reduce stock below cart quantity
            _cart.AddItem(_bookA, 1);
            _bookA.UpdateStock(0); // now stock drops to 0 after item was added to cart

            Invoice? invoice = _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            Assert.IsNull(invoice);
        }

        [TestMethod]
        public void ProcessCheckout_BookBecomesUnavailable_ReturnsNull()
        {
            _cart.AddItem(_bookA, 1);
            _bookA.Disable(); // book disabled after being added to cart

            Invoice? invoice = _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            Assert.IsNull(invoice);
        }

        [TestMethod]
        public void ProcessCheckout_Failure_DoesNotDeductStock()
        {
            int originalStock = _bookA.StockQuantity;
            _cart.AddItem(_bookA, 1);
            _bookA.Disable();

            _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            // Stock should be unchanged since checkout failed
            Assert.AreEqual(originalStock, _bookA.StockQuantity);
        }

        [TestMethod]
        public void ProcessCheckout_Failure_DoesNotSaveOrder()
        {
            _checkout.ProcessCheckout(_cart, "test@test.com", "123 St"); // empty cart
            Assert.AreEqual(0, _db.GetAllOrders().Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessCheckout_NullCart_ThrowsArgumentNullException()
        {
            _checkout.ProcessCheckout(null!, "test@test.com", "123 St");
        }

        // ── ProcessPaymentStub ────────────────────────────────────────────────

        [TestMethod]
        public void ProcessPaymentStub_UpdatesOrderStatusToPaid()
        {
            _cart.AddItem(_bookA, 1);
            Invoice? invoice = _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            Assert.IsNotNull(invoice);

            _checkout.ProcessPaymentStub(invoice!.Order, invoice);
            Assert.AreEqual(OrderStatus.Paid, invoice.Order.Status);
        }

        [TestMethod]
        public void ProcessPaymentStub_MarksInvoiceAsSettled()
        {
            _cart.AddItem(_bookA, 1);
            Invoice? invoice = _checkout.ProcessCheckout(_cart, "test@test.com", "123 St");
            Assert.IsNotNull(invoice);
            Assert.IsFalse(invoice!.IsSettled);

            _checkout.ProcessPaymentStub(invoice.Order, invoice);
            Assert.IsTrue(invoice.IsSettled);
        }

        // ── Multiple orders ───────────────────────────────────────────────────

        [TestMethod]
        public void ProcessCheckout_TwoOrders_BothSavedToDatabase()
        {
            _cart.AddItem(_bookA, 1);
            _checkout.ProcessCheckout(_cart, "customer1@test.com", "Addr 1");

            _cart.AddItem(_bookA, 1);
            _checkout.ProcessCheckout(_cart, "customer2@test.com", "Addr 2");

            Assert.AreEqual(2, _db.GetAllOrders().Count);
        }

        [TestMethod]
        public void ProcessCheckout_StockReducesAcrossMultipleOrders()
        {
            // Stock starts at 5
            _cart.AddItem(_bookA, 2);
            _checkout.ProcessCheckout(_cart, "a@test.com", "Addr A");
            Assert.AreEqual(3, _bookA.StockQuantity);

            _cart.AddItem(_bookA, 3);
            _checkout.ProcessCheckout(_cart, "b@test.com", "Addr B");
            Assert.AreEqual(0, _bookA.StockQuantity);
            Assert.IsFalse(_bookA.IsAvailable);
        }
    }
}
