using BookstoreApp.Models;

namespace BookstoreApp.UI;

public class BookDetailUI
{
    public void Display(Book book)
    {
        Console.Clear();
        Console.WriteLine("=== Book Details === ");
        Console.WriteLine();

        Console.WriteLine($"Title:      {book.Title}");
        Console.WriteLine($"Author:     {book.Author}");
        Console.WriteLine($"Genre:      {book.Genre}");
        Console.WriteLine($"ISBN:       {book.ISBN}");
        Console.WriteLine($"Price:      {book.Price:C}");
        Console.WriteLine($"Stock:      {book.StockQuantity}");
        Console.WriteLine();

        Console.WriteLine("Description:");
        Console.WriteLine(book.Description);

        Console.WriteLine();
        Console.WriteLine("Press Enter to return.");
        Console.ReadLine();
    }
}