using BookstoreApp.Data;
using BookstoreApp.Models;

namespace BookstoreApp.UI
{
    public class AccountUI
    {
        private readonly Database accountDB;
        private readonly string accountEmail;
        private readonly string accountName;
        private string accountAddress;

        public AccountUI(Database db, string email, string name, string address)
        {
            accountDB = db;
            accountEmail = email;
            accountName = name;
            accountAddress = address;
        }

        // Called from Program.cs — returns address in case it was updated.
        public string Show()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n  ╔══════════════════════════════════════╗");
                Console.WriteLine("  ║            MY ACCOUNT                ║");
                Console.WriteLine("  ╠══════════════════════════════════════╣");
                Console.WriteLine("  ║  1. View account details             ║");
                Console.WriteLine("  ║  2. Update delivery address          ║");
                Console.WriteLine("  ║  3. Change password                  ║");
                Console.WriteLine("  ║  4. Forgot / reset password          ║");
                Console.WriteLine("  ║  0. Back                             ║");
                Console.WriteLine("  ╚══════════════════════════════════════╝");
                Console.Write("  Choose an option: ");

                string? input = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (input)
                {
                    case "1": ViewDetails(); break;
                    case "2": UpdateAddress(); break;
                    case "3": ChangePassword(); break;
                    case "4": ForgotPassword(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("  [!] Invalid option. Please try again."); break;
                }
            }

            return accountAddress;
        }

        private void ViewDetails()
        {
            Console.WriteLine("  === ACCOUNT DETAILS ===");
            Console.WriteLine($"  Name    : {accountName}");
            Console.WriteLine($"  Email   : {accountEmail}");
            Console.WriteLine($"  Address : {(string.IsNullOrWhiteSpace(accountAddress) ? "(not set)" : accountAddress)}");
        }

        private void UpdateAddress()
        {
            Console.WriteLine("  === UPDATE DELIVERY ADDRESS ===");
            Console.WriteLine($"  Current address: {(string.IsNullOrWhiteSpace(accountAddress) ? "(not set)" : accountAddress)}");
            Console.Write("  New address (Enter to cancel): ");

            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("  No changes made.");
                return;
            }

            accountAddress = input;
            accountDB.UpdateUserAddress(accountEmail, accountAddress);
            Console.WriteLine("Delivery address updated.");
        }

        private void ChangePassword()
        {
            Console.WriteLine("  === CHANGE PASSWORD ===");
            Console.Write("  Current password: ");
            string? current = Console.ReadLine()?.Trim();

            if (!accountDB.CheckPassword(accountEmail, current ?? ""))
            {
                Console.WriteLine("Current password is incorrect!!!");
                return;
            }

            Console.Write("  New password (min 8 characters): ");
            string? newPassword = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                Console.WriteLine("  [!] Password must be at least 8 characters.");
                return;
            }

            Console.Write("  Confirm new password: ");
            string? confirm = Console.ReadLine()?.Trim();

            if (newPassword != confirm)
            {
                Console.WriteLine("  [!] Passwords do not match.");
                return;
            }

            accountDB.UpdateUserPassword(accountEmail, newPassword);
            Console.WriteLine("  [✓] Password changed successfully.");
        }

        private void ForgotPassword()
        {
            Console.WriteLine("  === RESET PASSWORD ===");
            Console.Write("  Enter your registered email: ");
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input) || !input.Contains('@'))
            {
                Console.WriteLine("  [!] Please enter a valid email address.");
                return;
            }

            Console.WriteLine($"  If an account with '{input}' exists, a reset link has been sent.");
            Console.WriteLine("  [Note: Email sending is not implemented in this prototype.]");
        }
    }
}
