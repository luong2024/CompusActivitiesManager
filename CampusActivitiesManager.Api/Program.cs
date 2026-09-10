using FirebaseAdmin;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS for MAUI and Web client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Initialize Firebase App safely if credentials exist
if (FirebaseApp.DefaultInstance == null)
{
    try
    {
        FirebaseApp.Create();
        Console.WriteLine("Firebase initialized successfully using Application Default Credentials.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Firebase initialization skipped or not configured: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run("http://localhost:5000");
