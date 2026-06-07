namespace BookstoreApp.Models
{
    public class Invoice
    {
        private static int _nextInvoiceId = 5000;

        public int InvoiceId { get; private set; }
        public Order Order { get; private set; }
        public DateTime IssuedDate { get; private set; }
        public decimal AmountDue { get; private set; }
        public bool IsSettled { get; private set; }

        public Invoice(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            InvoiceId = _nextInvoiceId++;
            Order = order;
            IssuedDate = DateTime.Now;
            AmountDue = order.CalculateTotal();
            IsSettled = false;
        }

        public void MarkAsSettled() => IsSettled = true;

        public void Display()
        {
            Console.WriteLine($"\n  Invoice #{InvoiceId} | Order #{Order.OrderId}");
            Console.WriteLine($"  Issued: {IssuedDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"  Bill To: {Order.CustomerEmail}");
            foreach (var line in Order.Lines)
                Console.WriteLine(line.ToString());
            Console.WriteLine($"  AMOUNT DUE: ${AmountDue:F2}");
            Console.WriteLine($"  Status: {(IsSettled ? "PAID" : "PENDING PAYMENT")}");
        }

        public static void ResetIdCounter(int startFrom = 5000) =>
            _nextInvoiceId = startFrom;
    }
}
