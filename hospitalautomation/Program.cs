using hospitalautomation.Models.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext Servisini Ekliyoruz
builder.Services.AddDbContext<ApplicationDbContext>();

// Authentication Servisini Ekliyoruz
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Giriş yapılmamışsa yönlendirilecek sayfa
        options.AccessDeniedPath = "/Login/AccessDenied"; // Yetkisiz erişim
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Oturum süresi
    });

// MVC Controller Servisini Ekliyoruz
builder.Services.AddControllersWithViews();

// Uygulama Yapılandırılıyor
var app = builder.Build();

// Middleware Konfigürasyonu
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication ve Authorization Middleware'lerini Ekliyoruz
app.UseAuthentication();
app.UseAuthorization();

// Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

// Uygulama Başlatılıyor
app.Run();
