using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
