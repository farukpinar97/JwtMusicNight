using JwtMusicNight.WebUI.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Tüm sayfalar varsayılan olarak giriş ister ([AllowAnonymous] olanlar hariç)
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

// Cookie oturumu (JWT claim olarak saklanır)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Error/403";   // admin olmayan biri /Admin'e girerse
        options.Cookie.Name = "JwtMusicNight.Auth";
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = false;
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtTokenHandler>();

builder.Services.AddHttpClient("MusicApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
    client.Timeout = TimeSpan.FromMinutes(2);       // büyük MP3 yüklemeleri için
})
.AddHttpMessageHandler<JwtTokenHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Admin area: /Admin/Dashboard, /Admin/Songs/Edit/5 ...
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Music}/{action=Index}/{id?}");

app.Run();
