using Azure.Storage.Blobs;
using Job_Recruitment.Contexts;
using Job_Recruitment.Helpers;
using Job_Recruitment.Interfaces;
using Job_Recruitment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<JobDbContext>().AddDefaultTokenProviders().AddDefaultUI();
builder.Services.AddDbContext<JobDbContext>(contextOptions => contextOptions.UseSqlServer(
    builder.Configuration.GetConnectionString("DatabaseConn")));
builder.Services.AddSingleton<IAzureBlobService, AzureBlobService>();
builder.Services.AddAuthentication()
.AddCookie()
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.MapRazorPages();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Jobs}/{action=Index}/{id?}");

app.Run();
