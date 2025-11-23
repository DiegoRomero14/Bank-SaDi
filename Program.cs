using Bank_SaDi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Setting the database context
builder.Services.AddDbContext<BankSaDiDbContext>(options =>
    options.UseSqlServer("Server=DESKTOP-M8IQAG5\\SQLEXPRESS;Database=BankSaDiDB;Integrated Security=True;Persist Security Info=False;TrustServerCertificate=True"));

builder.Services.AddDistributedMemoryCache(); // Usado para almacenar datos en la memoria del servidor.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión.
    options.Cookie.HttpOnly = true; // Solo se accede a la cookie desde el servidor.
    options.Cookie.IsEssential = true; // Necesaria para que la sesión funcione sin cookies opcionales.
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
