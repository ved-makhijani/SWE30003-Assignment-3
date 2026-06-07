using BookstoreApp.Models;

namespace BookstoreApp.UI
{
    public class AdminCatalogueUI
    {
        private readonly Catalogue _catalogue;

        public AdminCatalogueUI(Catalogue catalogue)
        {
            _catalogue = catalogue;
        }

        public void Show()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n=== ADMIN CATALOGUE ===");
                Console.WriteLine("1. View Catalogue");
                Console.WriteLine("2. Add Book");
                Console.WriteLine("3. Edit Book");
                Console.WriteLine("4. Disable Book");
                Console.WriteLine("0. Back");

                Console.Write("Choice: ");
                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ViewCatalogue();
                        break;

                    case "2":
                        AddBook();
                        break;

                    case "3":
                        EditBook();
                        break;

                    case "4":
                        DisableBook();
                        break;

                    case "0":
                        running = false;
                        break;
                }
            }
        }

        private void ViewCatalogue()
        {
            foreach (var book in _catalogue.ListAllBooks())
                Console.WriteLine(book);
        }

        private void AddBook()
        {
            Console.Write("ISBN: ");
            string isbn = Console.ReadLine() ?? "";

            if (_catalogue.ISBNExists(isbn))
            {
                Console.WriteLine("Duplicate ISBN.");
                return;
            }

            Console.Write("Title: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("Author: ");
            string author = Console.ReadLine() ?? "";

            Console.Write("Genre: ");
            string genre = Console.ReadLine() ?? "";

            Console.Write("Description: ");
            string desc = Console.ReadLine() ?? "";

            Console.Write("Price: ");
            decimal price = decimal.Parse(Console.ReadLine() ?? "0");

            Console.Write("Stock: ");
            int stock = int.Parse(Console.ReadLine() ?? "0");

            Book book = new Book(
                isbn,
                title,
                author,
                genre,
                desc,
                price,
                stock);

            _catalogue.AddBook(book);

            Console.WriteLine("Book added.");
        }

        private void EditBook()
        {
            Console.Write("ISBN: ");
            string isbn = Console.ReadLine() ?? "";

            Book? book = _catalogue.FindByISBN(isbn);

            if (book == null)
            {
                Console.WriteLine("Book not found.");
                return;
            }

            Console.Write("New Title: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("New Author: ");
            string author = Console.ReadLine() ?? "";

            Console.Write("New Genre: ");
            string genre = Console.ReadLine() ?? "";

            Console.Write("New Description: ");
            string desc = Console.ReadLine() ?? "";

            Console.Write("New Price: ");
            decimal price = decimal.Parse(Console.ReadLine() ?? "0");

            Console.Write("New Stock: ");
            int stock = int.Parse(Console.ReadLine() ?? "0");

            book.UpdateDetails(
                title,
                author,
                genre,
                desc,
                price);

            book.UpdateStock(stock);

            Console.WriteLine("Book updated.");
        }

        private void DisableBook()
        {
            Console.Write("ISBN: ");
            string isbn = Console.ReadLine() ?? "";

            if (_catalogue.DisableBook(isbn))
                Console.WriteLine("Book disabled.");
            else
                Console.WriteLine("Book not found.");
        }
    }
}