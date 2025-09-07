using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using QuestPDF.Infrastructure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web_Application_PFE.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Web_Application_PFE.Services.Interfaces;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
//QuestPDF.Settings.License = LicenseType.Community;


// Ajout des services de localisation
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        options.JsonSerializerOptions.WriteIndented = true;
    });


// Add services to the container.
builder.Services.AddRazorPages();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors());
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<JwtService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditService, AuditService>();
// Ajouter cette ligne dans les services
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;

    // Configuration des mots de passe
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 0;
    options.Password.RequiredUniqueChars = 0;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuration du service email
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, clsEmailConfirm>();
builder.Services.AddTransient<IEmailService, EmailService>();
// Pour autoriser les uploads de fichiers
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB max
});


builder.Services.AddSession();
builder.Services.AddMvc(options =>
{
    options.CacheProfiles.Add("NoCache", new CacheProfile()
    {
        NoStore = true,
        Location = ResponseCacheLocation.None,
        Duration = 0
    });
});

// Configuration JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddLogging(loggingBuilder => {
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();

    // D?sactivation compl?te du cache
    app.Use(async (context, next) =>
    {
        context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";
        await next();
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
QuestPDF.Settings.License = LicenseType.Community; // Version gratuite


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store");
        ctx.Context.Response.Headers.Append("Expires", "-1");
    }
});



app.UseRouting();
app.UseSession();
// Middleware de localisation (placer après UseRouting et avant UseAuthentication)


app.UseAuthentication();

app.UseAuthorization();
app.Use(async (context, next) =>
{
    var auditService = context.RequestServices.GetRequiredService<IAuditService>();
    await auditService.LogAsync("Requ?te", "HTTP", context.Request.Path,
        $"M?thode: {context.Request.Method}, Authentifi?: {context.User.Identity.IsAuthenticated}");
    await next();
});

app.Use(async (context, next) =>
{
    var auditService = context.RequestServices.GetRequiredService<IAuditService>();
    var userManager = context.RequestServices.GetRequiredService<UserManager<User>>();

    var user = await userManager.GetUserAsync(context.User);
    context.Items["CurrentUserId"] = user?.Id; // Stocke l'ID pour usage ult?rieur

    await next();
});

// Middleware pour g?rer JWT
app.Use(async (context, next) =>
{
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers["Authorization"] = $"Bearer {token}";
    }
    await next();
});
// Redirection automatique vers la page de connexion si l'utilisateur n'est pas authentifi?
app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated &&
        !context.Request.Path.StartsWithSegments("/Identity/Account/Login") &&
        !context.Request.Path.StartsWithSegments("/Identity/Account/ForgotPassword") &&
        !context.Request.Path.StartsWithSegments("/Identity/Account/ForgotPasswordConfirmation") &&
        !context.Request.Path.StartsWithSegments("/Identity/Account/ResetPassword")&&
        !context.Request.Path.StartsWithSegments("/Identity/Account/ResetPasswordConfirmation"))
    {
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }
    await next();
});

// Configuration des routes
app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapAreaControllerRoute(
    name: "Identity",
    areaName: "Identity",
    pattern: "Identity/{controller=Account}/{action=Login}/{id?}"); // Active les pages Razor, y compris la page de login

app.Run();
