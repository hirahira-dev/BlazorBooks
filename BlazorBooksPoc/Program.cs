using BlazorBooksPoc.Components;
using BlazorBooksPoc.Data;
using BlazorBooksPoc.Models;
using BlazorBooksPoc.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

builder.Services.AddScoped<IBookService, BookService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

SeedSampleData(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

void SeedSampleData(IHost appHost)
{
    using var scope = appHost.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (db.Books.Any())
    {
        return;
    }

    var now = DateTime.Now;
    db.Books.AddRange(
        new Book
        {
            Title = "Blazor Getting Started",
            Author = "Taro Yamada",
            Memo = "Check component structure",
            IsRead = false,
            CreatedAt = now.AddDays(-2)
        },
        new Book
        {
            Title = "EF Core Guide",
            Author = "Hanako Sato",
            Memo = "Try CRUD with SQLite",
            IsRead = true,
            CreatedAt = now.AddDays(-5)
        },
        new Book
        {
            Title = "C# 10 Features",
            Author = "John Doe",
            Memo = "Summarize record structs",
            IsRead = true,
            CreatedAt = now.AddDays(-10)
        }
    );

    db.SaveChanges();
}
