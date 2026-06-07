using BookstoreApp.Models;

namespace BookstoreApp.Data
{
    public class Database
    {
        private static Database? _instance;
        private static readonly object _lock = new();

        public static Database GetInstance()
        {
            if (_instance == null)
                lock (_lock)
                    _instance ??= new Database();
            return _instance;
        }

        public static void Reset() => _instance = null;

        private readonly List<Book> _books = new();
        private readonly List<Order> _orders = new();
        private readonly List<Invoice> _invoices = new();
        private readonly List<(string Email, string Password, string Role, string Name, string Address)> _users = new();

        private Database() { }

        public bool SaveUser(string email, string password, string role, string name, string address = "")
        {
            if (_users.Any(u => u.Email.ToLower() == email.ToLower())) return false;
            _users.Add((email.ToLower(), password, role, name, address));
            return true;
        }

        public bool EmailExists(string email) =>
            _users.Any(u => u.Email == email.ToLower());

        public (string Email, string Password, string Role, string Name, string Address)?
            GetUserByCredentials(string email, string password)
        {
            var u = _users.FirstOrDefault(u =>
                u.Email == email.ToLower() && u.Password == password);
            return u.Email != null ? u : null;
        }

        public void SaveBook(Book book)
        {
            if (!_books.Any(b => b.ISBN == book.ISBN)) _books.Add(book);
        }

        public List<Book> GetAllBooks() => new List<Book>(_books);

        public void SaveOrder(Order order) => _orders.Add(order);

        public List<Order> GetOrdersByCustomer(string email) =>
            _orders.Where(o => o.CustomerEmail.ToLower() == email.ToLower()).ToList();

        public List<Order> GetAllOrders() => new List<Order>(_orders);

        public bool IsBookInActiveOrders(string isbn)
        {
            var active = new[] { OrderStatus.PendingPayment, OrderStatus.Paid, OrderStatus.Packaged };
            return _orders.Any(o => active.Contains(o.Status) && o.Lines.Any(l => l.Book.ISBN == isbn));
        }

        public void SaveInvoice(Invoice invoice) => _invoices.Add(invoice);
        public List<Invoice> GetAllInvoices() => new List<Invoice>(_invoices);

        public void Seed()
        {
            SaveUser("admin@favouritebooks.com", "Admin1234", "Administrator", "Store Admin");

            var books = new List<Book>
            {
                new("978-0-06-112008-4", "To Kill a Mockingbird", "Harper Lee", "Fiction", "A tale of racial injustice.", 19.99m, 10),
                new("978-0-7432-7356-5", "The Great Gatsby", "F. Scott Fitzgerald", "Fiction", "The American dream.", 14.99m, 8),
                new("978-0-14-028329-7", "1984", "George Orwell", "Dystopian", "A totalitarian future.", 16.99m, 15),
                new("978-0-14-303943-3", "Brave New World", "Aldous Huxley", "Dystopian", "A pleasure-driven society.", 17.99m, 12),
                new("978-0-7432-7357-2", "The Catcher in the Rye", "J.D. Salinger", "Fiction", "Teenage alienation.", 15.99m, 7),
                new("978-0-06-251587-1", "Sapiens", "Yuval Noah Harari", "Non-Fiction", "A history of humankind.", 24.99m, 20),
                new("978-1-5011-6922-0", "Atomic Habits", "James Clear", "Self-Help", "Tiny changes, big results.", 22.99m, 18),
                new("978-0-525-55360-5", "The Midnight Library", "Matt Haig", "Fiction", "Between life and death.", 21.99m, 9),
            };

            foreach (var book in books) SaveBook(book);
        }
    }
}
