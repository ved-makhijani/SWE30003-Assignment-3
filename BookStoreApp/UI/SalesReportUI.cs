using BookstoreApp.Data;
using BookstoreApp.Models;


namespace BookstoreApp.UI
{
    public class SalesReportUI
    {
        private readonly SalesReport _report;

        public SalesReportUI(Database db)
        {
            _report = new SalesReport(db);
        }

        public void Show()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n=== SALES REPORT ===");
                Console.WriteLine("1. View Revenue");
                Console.WriteLine("2. View Order Count");
                Console.WriteLine("3. Export Report");
                Console.WriteLine("0. Back");

                Console.Write("Choice: ");
                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.WriteLine(
                            $"Total Revenue: ${_report.CalculateRevenue():F2}");
                        break;

                    case "2":
                        Console.WriteLine(
                            $"Total Orders: {_report.GetTotalOrders()}");
                        break;

                    case "3":
                        ExportReport();
                        break;

                    case "0":
                        running = false;
                        break;
                }
            }
        }

        private void ExportReport()
        {
            string report =
                $"Revenue: ${_report.CalculateRevenue():F2}\n" +
                $"Orders : {_report.GetTotalOrders()}";

            File.WriteAllText("SalesReport.txt", report);

            Console.WriteLine(
                "Report exported to SalesReport.txt");
        }
    }
}