using Microsoft.EntityFrameworkCore;
using ECommerceApp.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Capture connection string once at startup
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
Console.WriteLine($"🔍 DATABASE_URL found: {!string.IsNullOrEmpty(connectionString)}");

if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("⚠️ Using fallback connection from appsettings.json");
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("❌ No database connection string found.");
}

Console.WriteLine($"🔍 Connection string length: {connectionString.Length}");
Console.WriteLine($"🔍 Connection string preview: {connectionString.Substring(0, Math.Min(50, connectionString.Length))}...");

// Handle postgres:// vs postgresql://
if (connectionString.StartsWith("postgres://"))
{
    connectionString = connectionString.Replace("postgres://", "postgresql://");
    Console.WriteLine("🔄 Converted postgres:// to postgresql://");
}

// Test the connection string format before using it
try
{
    var testUri = new Uri(connectionString);
    Console.WriteLine($"✅ Connection string is valid URI format");
    Console.WriteLine($"🔍 Host: {testUri.Host}");
    Console.WriteLine($"🔍 Database: {testUri.AbsolutePath.TrimStart('/')}");
    Console.WriteLine($"🔍 Username: {testUri.UserInfo?.Split(':')[0] ?? "unknown"}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Connection string is NOT valid URI: {ex.Message}");
    Console.WriteLine($"🔍 Raw string: '{connectionString}'");
}

// Store the connection string in a variable that won't change
var finalConnectionString = connectionString;

// Add Entity Framework with captured connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    try
    {
        options.UseNpgsql(finalConnectionString);
        Console.WriteLine("✅ DbContext configured successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ DbContext configuration failed: {ex.Message}");
        Console.WriteLine($"🔍 Exception type: {ex.GetType().Name}");
        throw;
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "https://*.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

var app = builder.Build();

// Test database connection with direct query
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    Console.WriteLine("🔄 Testing database connection with direct query...");

    // Skip CanConnectAsync() and try direct database operations
    await context.Database.EnsureCreatedAsync();
    Console.WriteLine("✅ Database schema ensured!");

    var productCount = await context.Products.CountAsync();
    Console.WriteLine($"✅ Database query successful! Found {productCount} products.");
    Console.WriteLine("🎉 Database connection is working perfectly!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database connection error: {ex.Message}");
    Console.WriteLine($"🔍 Error type: {ex.GetType().Name}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"🔍 Inner error: {ex.InnerException.Message}");
    }
}

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API V1");
    c.RoutePrefix = "swagger";
});

// Middleware pipeline
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Configure port
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
var urls = $"http://0.0.0.0:{port}";
app.Urls.Add(urls);

app.Run();
