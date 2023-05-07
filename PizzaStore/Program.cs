using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var baglanti = builder.Configuration.GetConnectionString("ConString");
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(baglanti));

//****************************************
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
    {
        option.Cookie.Name = "Pizza.Auth";
        option.LoginPath= "/";
        option.AccessDeniedPath= "/";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });
//*************************************************
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.Cookie.Name = "Pizza.User";
  option.IdleTimeout = TimeSpan.FromMinutes(10);
    option.Cookie.Path = "/";
    
});


//************************************************



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pizzas}/{action=Index}/{id?}");

app.Run();
