namespace BookstoreApp.Models
{
    public static class OrderStatusNotifier
    {
        public static void NotifyCustomer(
            string email,
            string status)
        {
            Console.WriteLine(
                $"Notification sent to {email}: Order status updated to {status}");
        }
    }
}