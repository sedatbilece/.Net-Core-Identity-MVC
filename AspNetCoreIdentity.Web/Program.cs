using AspNetCoreIdentity.Web.Models;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentity.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentity.Web.OptionsModel;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreIdentity.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

//usertest

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddIdentityExtension();// Extensions/StartupExtensions (Sadeleþtirme için)

builder.Services.AddScoped<EmailService, EmailService>();

builder.Services.ConfigureApplicationCookie(opt =>
{

    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "IdentityApp";

    opt.LoginPath = "/Home/SignIn";
    opt.LogoutPath = "/Member/LogOut";
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan= TimeSpan.FromDays(60);
    opt.SlidingExpiration = true;
});

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

app.UseAuthorization();



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
