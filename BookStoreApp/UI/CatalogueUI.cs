using BookstoreApp.Models;

namespace BookstoreApp.UI;

public class CatalogueUI
{
    private readonly Catalogue _catalogue;
    private readonly BookDetailUI _bookDetailUI;

    public CatalogueUI(Catalogue catalogue, BookDetailUI bookDetailUI)
    {
        _catalogue = catalogue;
        _bookDetailUI = bookDetailUI;
    }

    public void Show()
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== Browse Catalogue ===");
            Console.WriteLine("1. Display All Books");
            Console.WriteLine("2. Search by Keyword");
            Console.WriteLine("3. Filter by Genre");
            Console.WriteLine("4. Filter by Author");
            Console.WriteLine("5. Filter by Max Price");
            Console.WriteLine("0. Back to Main Menu");
            Console.WriteLine();

            Console.Write("Choose an Option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DisplayBooks(_catalogue.ListAvailableBooks());
                    break;

                case "2":
                    SearchByKeyword();
                    break;

                case "3":
                    FilterByGenre();
                    break;

                case "4":
                    FilterByAuthor();
                    break;

                case "5":
                    FilterByPrice();
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid Option. Please Select a Number from the Menu.");
                    Pause();
                    break;
            }
        }
    }

    public void SearchByKeyword()
    {
        Console.Clear();
        Console.WriteLine("=== Search Books ===");

        Console.Write("Enter Keyword: ");
        string? keyword = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            Console.WriteLine("Search keyword cannot be blank.");
            Pause();
            return;
        }

        List<Book> results = _catalogue.SearchBooks(keyword.Trim());
        DisplayBooks(results);
    }

    private void FilterByGenre()
    {
        Console.Clear();
        Console.WriteLine("=== Filter by Genre ===");

        Console.Write("Enter Genre: ");
        string? genre = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(genre))
        {
            Console.WriteLine("Genre cannot be blank.");
            Pause();
            return;
        }

        List<Book> results = _catalogue.FilterByGenre(genre.Trim());
        DisplayBooks(results);
    }

    private void FilterByAuthor()
    {
        Console.Clear();
        Console.WriteLine("=== Filter by Author ===");

        Console.Write("Enter author: ");
        string? author = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(author))
        {
            Console.WriteLine("Author cannot be blank.");
            Pause();
            return;
        }

        List<Book> results = _catalogue.FilterByAuthor(author.Trim());
        DisplayBooks(results);
    }

    private void FilterByPrice()
    {
        Console.Clear();
        Console.WriteLine("=== Filter by Max Price ===");

        Console.Write("Enter a Maximum Price: ");
        bool validMax = decimal.TryParse(Console.ReadLine(), out decimal maxPrice);

        if (!validMax)
        {
            Console.WriteLine("Prices must be valid numbers.");
            Pause();
            return;
        }

        if (maxPrice < 0)
        {
            Console.WriteLine("Prices cannot be negative.");
            Pause();
            return;
        }  

        List<Book> results = _catalogue.FilterByMaxPrice(maxPrice);
        DisplayBooks(results);
    }

    private void DisplayBooks(List <Book> books)
    {
        Console.Clear();
        Console.WriteLine("=== Catalogue Results ===");
        Console.WriteLine();

        if (books.Count == 0)
        {
            Console.WriteLine("No books found.");
            Pause();
            return;
        }

        Console.WriteLine("{0,-25} {1,-35} {2,-25} {3,-15} {4,10}", "ISBN", "Title", "Author", "Genre", "Price");
        Console.WriteLine(new string('-', 95));

        foreach (Book book in books)
        {
            Console.WriteLine("{0,-25} {1,-35} {2,-25} {3,-15} {4,10:C}", book.ISBN, Shorten(book.Title, 33), Shorten(book.Author, 23), Shorten(book.Genre, 13), book.Price);
        }

        Console.WriteLine();
        Console.Write("Enter an ISBN to view details or press Enter to go back: ");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        Book? selectedBook = books.FirstOrDefault(book => book.ISBN == input);

        if (selectedBook == null)
        {
            Console.WriteLine("Invalid ISBN. Please select a book from the displayed list.");
            Pause();
            return;
        }

        _bookDetailUI.Display(selectedBook);
    }

    private string Shorten(string text, int maxLength)
    {
        if (text.Length <= maxLength)
        {
            return text;
        }

        return text.Substring(0, maxLength - 3) + "...";
    }

    private void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to Continue.");
        Console.ReadLine();
    }
}