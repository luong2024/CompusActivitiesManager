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
    try 
    {
        string credentialPath = @"D:\keys\campusacmanage-df24bcb494c7.json";
        
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(credentialPath)
        });
        
        Console.WriteLine("Firebase initialized successfully.");
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
