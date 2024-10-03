using Microsoft.EntityFrameworkCore.SqlServer;
using Library_Shop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Library_Shop.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Додавання сервісів до контейнера
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Час очікування
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();

// Налаштування підключення до бази даних LibraryDbContext
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LibraryDbContext") ??
        throw new InvalidOperationException("Connection string 'LibraryDbContext' not found."),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    )
);

// Налаштування підключення до бази даних UserDbContext
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("UserDbContext") ??
        throw new InvalidOperationException("Connection string 'UserDbContext' not found."),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    )
);

// Налаштування підключення до бази даних OrderDbContext
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OrderDbContext") ??
        throw new InvalidOperationException("Connection string 'OrderDbContext' not found."),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    )
);



builder.Services.AddIdentity<Library_User, IdentityRole>()
        .AddEntityFrameworkStores<UserDbContext>()
        .AddDefaultTokenProviders();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider serviceProvider = scope.ServiceProvider;

    // Створення контексту бази даних LibraryDbContext і застосування EnsureCreated для створення бази даних, якщо її немає
    var libraryContext = serviceProvider.GetRequiredService<LibraryDbContext>();
    libraryContext.Database.EnsureCreated();

    // Застосування міграцій після EnsureCreated
    await libraryContext.Database.MigrateAsync();

    // Створення контексту бази даних UserDbContext і застосування EnsureCreated для створення бази даних, якщо її немає
    var userContext = serviceProvider.GetRequiredService<UserDbContext>();
    userContext.Database.EnsureCreated();

    // Застосування міграцій після EnsureCreated
    await userContext.Database.MigrateAsync();

    // Ініціалізація початкових даних для бібліотеки
    await SeedData.InitializeAsync(serviceProvider, app.Environment);
}

// Налаштування HTTP-запитів
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // Використовуємо сесію перед автентифікацією та авторизацією
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
