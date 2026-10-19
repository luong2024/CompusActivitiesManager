using CampusActivitiesManager.Api.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Sanitize GOOGLE_APPLICATION_CREDENTIALS if surrounding quotes are present in the environment variable
string? credPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")?.Trim('"', ' ');
if (string.IsNullOrEmpty(credPath) || !File.Exists(credPath))
{
    if (File.Exists(@"D:\keys\campusacmanage-df24bcb494c7.json"))
    {
        credPath = @"D:\keys\campusacmanage-df24bcb494c7.json";
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);
    }
    else
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", null);
        credPath = null;
    }
}

string firebaseProjectId = builder.Configuration["Firebase:ProjectId"]
    ?? Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT")?.Trim('"', ' ')
    ?? "campusacmanage";

bool isFirebaseConfigured = false;
if (!string.IsNullOrEmpty(credPath) && File.Exists(credPath))
{
    try
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            using var stream = File.OpenRead(credPath);
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromStream(stream),
                ProjectId = firebaseProjectId
            });
        }
        isFirebaseConfigured = FirebaseApp.DefaultInstance != null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Startup] Notice: Could not initialize Firebase Cloud: {ex.Message}");
        isFirebaseConfigured = false;
    }
}

// Add services to the container.
builder.Services.AddControllers();

// Configure custom model validation to suppress default 400 filter and return RFC 7807 formatted ApiErrorResponse
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Configure CORS for client applications (.NET MAUI & Web)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register DI Services: Firebase Cloud or Local In-Memory Fallback
if (isFirebaseConfigured)
{
    Console.WriteLine("[Startup] Running with Firebase Cloud integration.");
    builder.Services.AddScoped<IFirebaseAccountService, FirebaseAccountService>();
    builder.Services.AddScoped<IFirebaseEventService, FirebaseEventService>();
}
else
{
    Console.WriteLine("[Startup] Running in Local In-Memory Mode (No external Firebase keys needed). Seed accounts and events loaded.");
    builder.Services.AddSingleton<IFirebaseAccountService, InMemoryAccountService>();
    builder.Services.AddSingleton<IFirebaseEventService, InMemoryEventService>();
}

builder.Services.AddSingleton<IFirebaseAttendanceService, FirebaseAttendanceService>();

// Configure JWT Authentication (resilient symmetric validation with dev fallback)
string jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? builder.Configuration["Jwt:SecretKey"]
    ?? "CampusActivitiesManager_DefaultDevSecretKey_2026!#";
var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey.PadRight(32, '_')));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = symmetricKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RoleClaimType = "role",
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    mode = isFirebaseConfigured ? "Firebase Cloud" : "Local In-Memory Mode",
    firebaseConnected = isFirebaseConfigured,
    timestamp = DateTime.UtcNow
}));

app.MapControllers();

// Bootstrap master Admin account directly to storage/memory if needed
try
{
    using var scope = app.Services.CreateScope();
    var accountService = scope.ServiceProvider.GetRequiredService<IFirebaseAccountService>();
    await accountService.EnsureAdminAccountAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"[Startup] EnsureAdminAccountAsync notice: {ex.Message}");
}

app.Run();

// Make Program class accessible for Integration Tests (WebApplicationFactory<Program>)
public partial class Program { }
