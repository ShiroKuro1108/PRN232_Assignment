# E-Commerce Clothing Store

A full-stack web application for selling clothing products built with .NET 8 Web API and vanilla JavaScript frontend.

## Features

### Backend API (CRUD Operations)
- **GET /api/products** - List all products
- **GET /api/products/{id}** - Get a single product
- **POST /api/products** - Create a new product
- **PUT /api/products/{id}** - Update a product
- **DELETE /api/products/{id}** - Delete a product

### Frontend UI
- Homepage with product listing
- Product detail pages
- Create/Edit product forms
- Delete product functionality
- Search and filter products
- Responsive design with Bootstrap

### Product Model
Each product includes:
- Name (required)
- Description (required)
- Price (required, must be > 0)
- Image URL (optional)
- Created/Updated timestamps

## Tech Stack

- **Backend**: .NET 8 Web API
- **Database**: PostgreSQL with Entity Framework Core
- **Frontend**: HTML, CSS, JavaScript, Bootstrap 5
- **ORM**: Entity Framework Core
- **API Documentation**: Swagger/OpenAPI

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- PostgreSQL database

### Database Setup
1. Install PostgreSQL and create a database named `ecommerce_db`
2. Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=ecommerce_db;Username=your_username;Password=your_password"
     }
   }
   ```

### Running the Application
1. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

2. Create and run database migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Open your browser and navigate to:
   - Frontend: `https://localhost:7xxx` (check console for exact port)
   - API Documentation: `https://localhost:7xxx/swagger`

## Project Structure

```
assignment/
├── Controllers/
│   ├── ProductsController.cs      # API endpoints
│   └── WeatherForecastController.cs
├── Data/
│   └── ApplicationDbContext.cs    # Database context
├── DTOs/
│   └── ProductDto.cs             # Data transfer objects
├── Models/
│   └── Product.cs                # Product entity
├── wwwroot/
│   ├── index.html               # Frontend HTML
│   ├── app.js                   # Frontend JavaScript
│   └── styles.css               # Frontend CSS
├── Program.cs                   # Application configuration
└── appsettings.json            # Configuration settings
```

## API Endpoints

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Request/Response Examples

**Create Product (POST /api/products):**
```json
{
  "name": "Classic T-Shirt",
  "description": "Comfortable cotton t-shirt",
  "price": 19.99,
  "image": "https://example.com/image.jpg"
}
```

**Product Response:**
```json
{
  "id": 1,
  "name": "Classic T-Shirt",
  "description": "Comfortable cotton t-shirt",
  "price": 19.99,
  "image": "https://example.com/image.jpg",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

## Deployment

The application is ready for deployment to platforms like:
- **Vercel** (for frontend + API)
- **Render** (for full-stack deployment)
- **Azure App Service**
- **Heroku**

For database, use:
- **PostgreSQL on Render**
- **Azure Database for PostgreSQL**
- **AWS RDS**
- **MongoDB Atlas** (if switching to MongoDB)

## Security Notes

- Connection strings and sensitive data should be stored in environment variables
- Use proper authentication/authorization for production
- Implement input validation and sanitization
- Use HTTPS in production
- Consider rate limiting for API endpoints
