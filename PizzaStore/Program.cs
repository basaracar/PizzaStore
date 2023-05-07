using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Data;

var builder = WebApplication.CreateBuilder(args);

// Servislerin konteyn�rdeki kullan�m�n� ekler.
// Ba�lant� dizesi kullanarak veritaban� i�lemleri ger�ekle�tirilir.
var baglanti = builder.Configuration.GetConnectionString("ConString");
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(baglanti));

// Kimlik do�rulama i�in bir �erez ekler. Kullan�c�lar oturum a�arken bu kullan�l�r.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.Cookie.Name = "Pizza.Auth";
    option.LoginPath = "/";
    option.AccessDeniedPath = "/";
    option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

// Oturum y�netimi i�in servis ekler.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.Cookie.Name = "Pizza.User";
    option.IdleTimeout = TimeSpan.FromMinutes(10);
    option.Cookie.Path = "/";
});

var app = builder.Build();

// HTTP istek boru hatt�n� yap�land�r�r.
// Hata y�netimi ve HTTPS y�nlendirmesi gibi i�lemler ger�ekle�tirilir.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Varsay�lan HSTS de�eri 30 g�nd�r. Bu, �retim senaryolar� i�in de�i�tirilebilir.
    app.UseHsts();
}

// HTTPS y�nlendirmesi ger�ekle�tirilir.
app.UseHttpsRedirection();

// Statik dosyalar� sunmak i�in kullan�l�r.
app.UseStaticFiles();

app.UseRouting();

// Kimlik do�rulama i�lemleri i�in kullan�l�r.
app.UseAuthentication();

// Yetkilendirme i�lemleri i�in kullan�l�r.
app.UseAuthorization();

// Oturum verilerinin kullan�labilmesi i�in kullan�l�r.
app.UseSession();

// Denetleyici rotas�n� yap�land�r�r.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pizzas}/{action=Index}/{id?}");

app.Run();