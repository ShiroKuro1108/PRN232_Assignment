using Microsoft.EntityFrameworkCore;
using ECommerceApp.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Debug environment variables first
Console.WriteLine("=== DATABASE CONNECTION DEBUG ===");
var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
Console.WriteLine($"DATABASE_URL exists: {!string.IsNullOrEmpty(dbUrl)}");

if (!string.IsNullOrEmpty(dbUrl))
{
    Console.WriteLine($"DATABASE_URL length: {dbUrl.Length}");
    Console.WriteLine($"DATABASE_URL preview: {dbUrl.Substring(0, Math.Min(30, dbUrl.Length))}...");
}
Console.WriteLine("=====================================");

// Add Entity Framework with robust connection handling
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (string.IsNullOrEmpty(connectionString))
    {
        // Fallback to appsettings.json for local development
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine("Using fallback connection string from appsettings.json");
    }

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("No database connection string found. Set DATABASE_URL environment variable.");
    }

    // Clean up the connection string
    connectionString = connectionString.Trim();

    // Handle postgres:// vs postgresql:// 
    if (connectionString.StartsWith("postgres://"))
    {
        connectionString = connectionString.Replace("postgres://", "postgresql://");
        Console.WriteLine("Converted postgres:// to postgresql://");
    }

    Console.WriteLine($"Final connection string preview: {connectionString.Substring(0, Math.Min(30, connectionString.Length))}...");

    try
    {
        options.UseNpgsql(connectionString);
        Console.WriteLine("DbContext configured successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DbContext configuration failed: {ex.Message}");
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
            // Add your Vercel URL here later
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API V1");
    c.RoutePrefix = "swagger";
});

// Test database connection with detailed error handling
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        Console.WriteLine("Testing database connection...");

        // Test the connection
        var canConnect = await context.Database.CanConnectAsync();

        if (canConnect)
        {
            Console.WriteLine("Database connection successful!");

            // Try to ensure database is created
            Console.WriteLine("Ensuring database schema exists...");
            var created = await context.Database.EnsureCreatedAsync();

            if (created)
            {
                Console.WriteLine("Database schema created successfully!");
            }
            else
            {
                Console.WriteLine("Database schema already exists");
            }
        }
        else
        {
            Console.WriteLine(" Cannot connect to database");
        }
    }
    catch (Npgsql.NpgsqlException ex)
    {
        Console.WriteLine($"PostgreSQL Error: {ex.Message}");
        Console.WriteLine($"Error Code: {ex.SqlState}");

        if (ex.Message.Contains("authentication"))
        {
            Console.WriteLine("This looks like an authentication issue. Check username/password in connection string.");
        }
        else if (ex.Message.Contains("does not exist"))
        {
            Console.WriteLine(" Database or host not found. Check hostname and database name.");
        }

        throw;
    }
    catch (System.ArgumentException ex)
    {
        Console.WriteLine($" Connection String Format Error: {ex.Message}");
        Console.WriteLine(" The connection string format is invalid. Check for typos or missing parts.");
        throw;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected database error: {ex.Message}");
        throw;
    }
}

// Middleware pipeline
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Configure port
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
var urls = $"http://0.0.0.0:{port}";
app.Urls.Add(urls);

Console.WriteLine($"Application starting on: {urls}");

app.Run();