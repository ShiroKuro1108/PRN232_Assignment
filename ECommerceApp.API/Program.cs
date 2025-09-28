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
    Console.WriteLine($"Raw DATABASE_URL: {(!string.IsNullOrEmpty(connectionString) ? "Found" : "Not Found")}");

    if (string.IsNullOrEmpty(connectionString))
    {
        // Fallback to appsettings.json for local development
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine("Using fallback connection string from appsettings.json");
        Console.WriteLine("This should only happen in local development!");
    }
    else
    {
        Console.WriteLine("Using DATABASE_URL environment variable");
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

    // Extract and log username for debugging (without exposing password)
    try
    {
        var uri = new Uri(connectionString);
        var username = uri.UserInfo?.Split(':')[0];
        Console.WriteLine($"Connection will use username: {username ?? "unknown"}");
        Console.WriteLine($"Database host: {uri.Host}");
        Console.WriteLine($"Database name: {uri.AbsolutePath.TrimStart('/')}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not parse connection string for debugging: {ex.Message}");
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

            // Test a simple query to verify everything works
            Console.WriteLine("Testing database query...");
            var productCount = await context.Products.CountAsync();
            Console.WriteLine($"Database query successful! Found {productCount} products.");
        }
        else
        {
            Console.WriteLine("❌ CanConnectAsync() returned false");

            // Try a direct query to see if connection actually works
            Console.WriteLine("🔄 Attempting direct database query despite CanConnectAsync() failure...");
            try
            {
                var productCount = await context.Products.CountAsync();
                Console.WriteLine($"🎉 SURPRISE! Direct query worked! Found {productCount} products.");
                Console.WriteLine("🔍 This means CanConnectAsync() is unreliable but database actually works!");
            }
            catch (Exception queryEx)
            {
                Console.WriteLine($"❌ Direct query also failed: {queryEx.Message}");
                Console.WriteLine("🔍 This confirms the database connection is truly broken");
                Console.WriteLine("🔍 Possible causes:");
                Console.WriteLine("   - Wrong credentials (username/password)");
                Console.WriteLine("   - Wrong hostname or port");
                Console.WriteLine("   - Database is not accessible from this network");
                Console.WriteLine("   - Database is sleeping (free tier limitation)");
            }
        }
    }
    catch (Npgsql.NpgsqlException ex)
    {
        Console.WriteLine($"PostgreSQL Error: {ex.Message}");
        Console.WriteLine($"Error Code: {ex.SqlState}");

        if (ex.Message.Contains("authentication") || ex.Message.Contains("password"))
        {
            Console.WriteLine("AUTHENTICATION ISSUE: Check username/password in connection string.");
            Console.WriteLine("   - Verify DATABASE_URL has correct credentials");
            Console.WriteLine("   - Check for typos in username or password");
        }
        else if (ex.Message.Contains("does not exist"))
        {
            Console.WriteLine("DATABASE/HOST NOT FOUND: Check hostname and database name.");
            Console.WriteLine("   - Verify hostname is correct");
            Console.WriteLine("   - Check database name matches exactly");
        }
        else if (ex.Message.Contains("timeout") || ex.Message.Contains("network"))
        {
            Console.WriteLine(" NETWORK ISSUE: Cannot reach database server.");
            Console.WriteLine("   - Database might be sleeping (free tier)");
            Console.WriteLine("   - Check if database is in same region");
        }

        // Don't throw in production - let app start anyway
        Console.WriteLine("App will continue starting despite database connection failure");
    }
    catch (System.ArgumentException ex)
    {
        Console.WriteLine($"Connection String Format Error: {ex.Message}");
        Console.WriteLine("The connection string format is invalid. Check for typos or missing parts.");
        Console.WriteLine("App will continue starting despite database connection failure");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected database error: {ex.Message}");
        Console.WriteLine($"Error type: {ex.GetType().Name}");
        Console.WriteLine("App will continue starting despite database connection failure");
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