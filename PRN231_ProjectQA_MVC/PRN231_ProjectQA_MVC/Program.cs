using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using PRN231_ProjectQA_MVC.Authorizations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

// Register the HttpClient service
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
   .AddCookie()
   .AddGoogle(options =>
   {
       options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
       options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
       options.Scope.Add("openid");
       options.Scope.Add("profile");
       options.Scope.Add("email");
       options.ClaimActions.MapJsonKey("picture", "picture", "url");
       options.CallbackPath = "/signin-google";
     
   });
builder.Services.AddHttpContextAccessor();

// Register the HttpClient service
builder.Services.AddHttpClient();

// Register AuthTokenHandler
builder.Services.AddTransient<AuthTokenHandler>();

// Register HttpClient and use AuthTokenHandler
builder.Services.AddHttpClient("MyHttpClient")
        .AddHttpMessageHandler<AuthTokenHandler>();
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
