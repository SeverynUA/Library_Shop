using Microsoft.EntityFrameworkCore.SqlServer;
using Library_Shop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Library_Shop.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// ��������� ������ �� ����������
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ��� ����������
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();

// ������������ ���������� �� ���� ����� LibraryDbContext
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LibraryDbContext") ??
        throw new InvalidOperationException("Connection string 'LibraryDbContext' not found."),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    )
);

// ������������ ���������� �� ���� ����� UserDbContext
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("UserDbContext") ??
        throw new InvalidOperationException("Connection string 'UserDbContext' not found."),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    )
);

// ������������ ���������� �� ���� ����� OrderDbContext
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

    // ��������� ��������� ���� ����� LibraryDbContext � ������������ EnsureCreated ��� ��������� ���� �����, ���� �� ����
    var libraryContext = serviceProvider.GetRequiredService<LibraryDbContext>();
    libraryContext.Database.EnsureCreated();

    // ������������ ������� ���� EnsureCreated
    await libraryContext.Database.MigrateAsync();

    // ��������� ��������� ���� ����� UserDbContext � ������������ EnsureCreated ��� ��������� ���� �����, ���� �� ����
    var userContext = serviceProvider.GetRequiredService<UserDbContext>();
    userContext.Database.EnsureCreated();

    // ������������ ������� ���� EnsureCreated
    await userContext.Database.MigrateAsync();

    // ����������� ���������� ����� ��� ��������
    await SeedData.InitializeAsync(serviceProvider, app.Environment);
}

// ������������ HTTP-������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // ������������� ���� ����� ��������������� �� ������������
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
