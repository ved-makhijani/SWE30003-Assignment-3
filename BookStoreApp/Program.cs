// Program.cs — Blazor Server entry point
// Replaces the console entry point with a Blazor Server web application.

using BookstoreApp.Data;
using BookstoreApp.Models;
using BookstoreApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Bootstrap the bookstore backend as singletons
// so every Blazor page shares the same data
builder.Services.AddSingleton<Database>(sp =>
{
    var db = Database.GetInstance();
    db.Seed();
    return db;
});

builder.Services.AddSingleton<BookstoreApp.Models.Catalogue>(sp =>
{
    var db = sp.GetRequiredService<Database>();
    var catalogue = new BookstoreApp.Models.Catalogue();
    foreach (var book in db.GetAllBooks())
        catalogue.AddBook(book);
    return catalogue;
});

builder.Services.AddSingleton<BookstoreApp.Models.Checkout>(sp =>
{
    var db = sp.GetRequiredService<Database>();
    var catalogue = sp.GetRequiredService<Catalogue>();
    return new BookstoreApp.Models.Checkout(db, catalogue);
});

// Session service is scoped — one per browser session (tab)
builder.Services.AddScoped<SessionService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
