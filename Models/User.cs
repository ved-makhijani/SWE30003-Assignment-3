namespace BookstoreApp.Models;

public abstract class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string DeliveryAddress { get; set; } = "";

    public string FullName => $"{FirstName} {LastName}";

    public virtual void UpdateAccountDetails()
    {
        Console.WriteLine("\n  === UPDATE ACCOUNT DETAILS ===");
        Console.WriteLine($"  Current name: {FullName}");

        Console.Write("  New first name (Enter to keep): ");
        string? input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input)) FirstName = input.Trim();

        Console.Write("  New last name (Enter to keep): ");
        input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input)) LastName = input.Trim();

        Console.Write("  New delivery address (Enter to keep): ");
        input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input)) DeliveryAddress = input.Trim();

        Console.WriteLine("  [✓] Account details updated.");
    }

    public virtual void DisplayDetails()
    {
        Console.WriteLine("  === ACCOUNT DETAILS ===");
        Console.WriteLine($"  Name    : {FullName}");
        Console.WriteLine($"  Email   : {Email}");
        Console.WriteLine($"  Address : {(string.IsNullOrWhiteSpace(DeliveryAddress) ? "(not set)" : DeliveryAddress)}");
        Console.WriteLine($"  Role    : {GetType().Name}");
    }
}

// Registered customer — can browse, place orders, manage their account.
public class Customer : User { }

// Administrator — full system access.
public class Administrator : User { }

// Store manager — catalogue and sales management access.
public class StoreManager : User { }

// Warehouse staff — order fulfilment access.
public class WarehouseStaff : User { }
