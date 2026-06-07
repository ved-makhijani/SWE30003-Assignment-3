using BookstoreApp.Data;

namespace BookstoreApp.Models
{
    public class SalesReport
    {
        private readonly Database _db;

        public SalesReport(Database db)
        {
            _db = db;
        }

        public decimal CalculateRevenue()
        {
            return _db.GetAllOrders()
                .Sum(o => o.CalculateTotal());
        }

        public int GetTotalOrders()
        {
            return _db.GetAllOrders().Count;
        }

        public List<Order> GetOrders()
        {
            return _db.GetAllOrders();
        }
    }
}