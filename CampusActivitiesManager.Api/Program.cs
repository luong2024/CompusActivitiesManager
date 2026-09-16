using CampusActivitiesManager.Api.Services;
using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

// Register Account & Auth Services (DI)
builder.Services.AddScoped<IFirebaseAccountService, FirebaseAccountService>();

// Configure Firebase JWT Authentication
string firebaseProjectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT") ?? "campusacmanage";
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
            RoleClaimType = "role" // Map Firebase custom claim "role" to .NET Role
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

// Initialize Firebase App
if (FirebaseApp.DefaultInstance == null)
{
    // Firebase Admin SDK will automatically look for the 
    // GOOGLE_APPLICATION_CREDENTIALS environment variable.
    try 
    {
        FirebaseApp.Create();
        app.Logger.LogInformation("Firebase initialized successfully using Application Default Credentials.");
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning("Could not initialize Firebase Default Instance: {Message}", ex.Message);
    }
}

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
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.MapControllers();

app.Run();

// Make Program class accessible for Integration Tests (WebApplicationFactory<Program>)
public partial class Program { }
