using Microsoft.AspNetCore.Authentication.Cookies;
using MVCWEB.Extensions.Services;

var builder = WebApplication.CreateBuilder(args);

// Add collection of services
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationServices(); //custom from extensions
builder.Services.AddRepositoryServices();  //custom from extensions

// TODO : Authentication config 
builder.Services.
    AddAuthentication(
    CookieAuthenticationDefaults
    .AuthenticationScheme)
    .AddCookie(options =>
        {
            options.LoginPath = "/auth/login";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            options.SlidingExpiration = true;
        }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
   
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
