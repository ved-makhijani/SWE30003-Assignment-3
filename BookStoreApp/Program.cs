using BookstoreApp.Data;
using BookstoreApp.Models;
using BookstoreApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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
    var catalogue = sp.GetRequiredService<BookstoreApp.Models.Catalogue>();
    return new BookstoreApp.Models.Checkout(db, catalogue);
});

builder.Services.AddScoped<SessionService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
