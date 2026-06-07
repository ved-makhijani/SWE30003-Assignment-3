namespace BookstoreApp.UI;

public class MainMenuUI
{
    private readonly CatalogueUI _catalogueUI;
    private readonly CartAndOrderUI _cartAndOrderUI;

    public MainMenuUI(CatalogueUI catalogueUI, CartAndOrderUI cartAndOrderUI)
    {
        _catalogueUI = catalogueUI;
        _cartAndOrderUI = cartAndOrderUI;
    }

    public void Show()
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine(" Favourite Books Online Bookstore");
            Console.WriteLine("===================================");
            Console.WriteLine("1. Browse Catalogue");
            Console.WriteLine("2. Shopping Cart / Checkout / Orders");
            Console.WriteLine("0. Exit");
            Console.WriteLine();

            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    _catalogueUI.Show(); 
                    break;

                case "2":
                    _cartAndOrderUI.Show();
                    break;


                case "0":
                    running = false;
                    Console.WriteLine("Thanks for visiting. Check back soon.");
                    break;

                default:
                    Console.WriteLine("Invalid Option. Please Select a Number from the Menu.");
                    Pause();
                    break;
            }
        }
    }

    private void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to Continue.");
        Console.ReadLine();
    }
}