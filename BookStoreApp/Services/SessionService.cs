// Services/SessionService.cs
// Scoped service that holds the current user's session state.
// Replaces the console's currentEmail/currentName/currentRole/cart variables.
// Scoped = one instance per browser connection (equivalent to one console session).

using BookstoreApp.Models;

namespace BookstoreApp.Services
{
    public class SessionService
    {
        // ── Logged-in user state ──────────────────────────────────────────────
        public string? Email { get; private set; }
        public string? Name { get; private set; }
        public string? Role { get; private set; }
        public string? DeliveryAddress { get; private set; }

        public bool IsLoggedIn => Email != null;
        public bool IsCustomer => Role == "Customer";
        public bool IsAdmin => Role == "Administrator" || Role == "StoreManager";

        // ── Shopping cart ─────────────────────────────────────────────────────
        public ShoppingCart Cart { get; private set; } = new ShoppingCart();

        // ── Login / Logout ────────────────────────────────────────────────────

        public void Login(string email, string name, string role, string address)
        {
            Email = email;
            Name = name;
            Role = role;
            DeliveryAddress = address;
            Cart = new ShoppingCart(); // fresh cart on login
        }

        public void Logout()
        {
            Email = null;
            Name = null;
            Role = null;
            DeliveryAddress = null;
            Cart = new ShoppingCart();
        }

        public void UpdateDeliveryAddress(string address)
        {
            DeliveryAddress = address;
        }

        // ── Notification (lets pages trigger UI refresh) ───────────────────────
        public event Action? OnChange;
        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
