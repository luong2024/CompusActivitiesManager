using CampusActivitiesManager.Api.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Sanitize GOOGLE_APPLICATION_CREDENTIALS if surrounding quotes are present in the environment variable
string? credPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")?.Trim('"', ' ');
if (!string.IsNullOrEmpty(credPath) && File.Exists(credPath))
{
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);
}

string firebaseProjectId = builder.Configuration["Firebase:ProjectId"]
    ?? Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT")?.Trim('"', ' ')
    ?? "campusacmanage";

// Initialize Firebase App early so DI services can access FirebaseAuth.DefaultInstance
if (FirebaseApp.DefaultInstance == null)
{
    try
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.GetApplicationDefault(),
            ProjectId = firebaseProjectId
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not initialize Firebase Default Instance: {ex.Message}");
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

// Register Firebase Services (DI)
builder.Services.AddScoped<IFirebaseAccountService, FirebaseAccountService>();
builder.Services.AddScoped<IFirebaseEventService, FirebaseEventService>();

// Configure Firebase JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true,
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", firebaseConnected = FirebaseApp.DefaultInstance != null, timestamp = DateTime.UtcNow }));

app.MapControllers();

app.Run();

// Make Program class accessible for Integration Tests (WebApplicationFactory<Program>)
public partial class Program { }
