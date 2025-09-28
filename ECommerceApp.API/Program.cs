using Microsoft.EntityFrameworkCore;
using ECommerceApp.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                          ?? builder.Configuration.GetConnectionString("DefaultConnection");

    // Handle Render PostgreSQL URL format
    if (connectionString != null && connectionString.StartsWith("postgres://"))
    {
        connectionString = connectionString.Replace("postgres://", "postgresql://");
    }

    options.UseNpgsql(connectionString);
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
            // Add your Vercel URL here later: "https://your-app.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for API testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API V1");
    c.RoutePrefix = "swagger";
});

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Creating database...");
        context.Database.EnsureCreated();
        logger.LogInformation("Database created successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while creating the database: {Message}", ex.Message);
    }
}

// Middleware pipeline
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Configure port - Render provides PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
var urls = $"http://0.0.0.0:{port}";
app.Urls.Add(urls);

Console.WriteLine($"Application starting on: {urls}");

app.Run();