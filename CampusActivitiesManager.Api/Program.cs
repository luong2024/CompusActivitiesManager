using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize Firebase App
if (FirebaseApp.DefaultInstance == null)
{
    // Firebase Admin SDK will automatically look for the 
    // GOOGLE_APPLICATION_CREDENTIALS environment variable.
    try 
    {
        FirebaseApp.Create();
        Console.WriteLine("Firebase initialized successfully using Application Default Credentials.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not initialize Firebase Default Instance: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
