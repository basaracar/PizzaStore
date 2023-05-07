using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Data;

var builder = WebApplication.CreateBuilder(args);

// Servislerin konteynýrdeki kullanýmýný ekler.
// Baðlantý dizesi kullanarak veritabaný iþlemleri gerçekleþtirilir.
var baglanti = builder.Configuration.GetConnectionString("ConString");
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(baglanti));

// Kimlik doðrulama için bir çerez ekler. Kullanýcýlar oturum açarken bu kullanýlýr.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.Cookie.Name = "Pizza.Auth";
    option.LoginPath = "/";
    option.AccessDeniedPath = "/";
    option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

// Oturum yönetimi için servis ekler.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.Cookie.Name = "Pizza.User";
    option.IdleTimeout = TimeSpan.FromMinutes(10);
    option.Cookie.Path = "/";
});

var app = builder.Build();

// HTTP istek boru hattýný yapýlandýrýr.
// Hata yönetimi ve HTTPS yönlendirmesi gibi iþlemler gerçekleþtirilir.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Varsayýlan HSTS deðeri 30 gündür. Bu, üretim senaryolarý için deðiþtirilebilir.
    app.UseHsts();
}

// HTTPS yönlendirmesi gerçekleþtirilir.
app.UseHttpsRedirection();

// Statik dosyalarý sunmak için kullanýlýr.
app.UseStaticFiles();

app.UseRouting();

// Kimlik doðrulama iþlemleri için kullanýlýr.
app.UseAuthentication();

// Yetkilendirme iþlemleri için kullanýlýr.
app.UseAuthorization();

// Oturum verilerinin kullanýlabilmesi için kullanýlýr.
app.UseSession();

// Denetleyici rotasýný yapýlandýrýr.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pizzas}/{action=Index}/{id?}");

app.Run();