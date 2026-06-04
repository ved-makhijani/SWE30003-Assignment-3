using BookstoreApp.Models;

namespace BookstoreApp.Models
{
    public enum OrderStatus
    {
        PendingPayment, Paid, Packaged, Dispatched, Delivered, Cancelled
    }

    public class Order
    {
        private static int _nextOrderId = 1000;

        public int OrderId { get; private set; }
        public string CustomerEmail { get; private set; }
        public string DeliveryAddress { get; private set; }
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

        private readonly List<OrderLine> _lines;

        public Order(string customerEmail, string deliveryAddress, List<CartItem> cartItems)
        {
            if (string.IsNullOrWhiteSpace(customerEmail))
                throw new ArgumentException("Customer email cannot be empty.");
            if (string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("Delivery address cannot be empty.");
            if (cartItems == null || cartItems.Count == 0)
                throw new ArgumentException("Cannot create an order from an empty cart.");

            OrderId = _nextOrderId++;
            CustomerEmail = customerEmail;
            DeliveryAddress = deliveryAddress;
            OrderDate = DateTime.Now;
            Status = OrderStatus.PendingPayment;
            _lines = cartItems.Select(c => new OrderLine(c.Book, c.Quantity)).ToList();
        }

        public decimal CalculateTotal() => _lines.Sum(l => l.LineTotal);

        public void UpdateStatus(OrderStatus newStatus) => Status = newStatus;

        public void DisplaySummary()
        {
            Console.WriteLine($"\n  Order #{OrderId} | {Status} | {OrderDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"  Deliver to: {DeliveryAddress}");
            foreach (OrderLine line in _lines)
                Console.WriteLine(line.ToString());
            Console.WriteLine($"  TOTAL: ${CalculateTotal():F2}");
        }

        public static void ResetIdCounter(int startFrom = 1000) =>
            _nextOrderId = startFrom;
    }
}
