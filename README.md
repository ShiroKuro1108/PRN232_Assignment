# E-Commerce Web Application - Assignment 2

A full-stack e-commerce web application built with **React** (frontend) and **ASP.NET Core Web API** (backend) featuring user authentication, product management, shopping cart, and order processing.

---

## 🚀 Features

### ✅ **Completed Features**

#### **1. Authentication & Authorization**
- ✅ User Registration with email and password
- ✅ User Login with JWT token authentication
- ✅ Logout functionality
- ✅ Protected routes and endpoints
- ✅ Only authenticated users can create/update/delete products
- ✅ Unauthenticated users can browse and view products

#### **2. Product Management (CRUD)**
- ✅ Create new products (authenticated users only)
- ✅ Read/view all products (public access)
- ✅ Update existing products (authenticated users only)
- ✅ Delete products (authenticated users only)
- ✅ Product model includes: name, description, price, image URL

#### **3. Shopping Cart**
- ✅ Add products to cart
- ✅ View cart items with quantity and total price
- ✅ Update item quantities
- ✅ Remove items from cart
- ✅ Clear entire cart
- ✅ Cart persists per user in database

#### **4. Order Management**
- ✅ Place orders from cart
- ✅ Orders saved in database
- ✅ Order model includes: userId, products, totalAmount, status
- ✅ View order history
- ✅ Order details with items and pricing
- ✅ Order status tracking (pending, paid, shipped, delivered, cancelled)

#### **5. UI Features**
- ✅ Homepage with product list grid
- ✅ Product detail page
- ✅ Create/Edit product forms (authentication required)
- ✅ Delete product confirmation modal
- ✅ Navigation menu with dynamic auth state
- ✅ Login & Register forms
- ✅ Logout button
- ✅ Cart page with item management
- ✅ Checkout page with order confirmation
- ✅ Order history page
- ✅ **Search/filter products** (bonus feature)
- ✅ Responsive design
- ✅ Cyberpunk-themed UI

---

## 📁 Project Structure

```
ECommerceApp/
├── ECommerceApp.API/              # Backend (ASP.NET Core Web API)
│   ├── Controllers/
│   │   ├── AuthController.cs     # Register, Login, Logout
│   │   ├── CartController.cs      # Cart CRUD operations
│   │   ├── OrdersController.cs    # Order management
│   │   └── ProductsController.cs  # Product CRUD operations
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── DTOs/
│   │   ├── AuthDtos.cs
│   │   ├── CartDtos.cs
│   │   └── OrderDtos.cs
│   ├── Models/
│   │   ├── Product.cs
│   │   ├── User.cs
│   │   ├── Cart.cs
│   │   └── Order.cs
│   ├── Services/
│   │   ├── PasswordHasher.cs
│   │   └── TokenService.cs
│   ├── Migrations/
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
└── client/                        # Frontend (React)
    ├── public/
    ├── src/
    │   ├── components/
    │   │   ├── Cart.js           # Shopping cart page
    │   │   ├── Checkout.js       # Checkout page
    │   │   ├── Orders.js         # Order history
    │   │   ├── Login.js          # Login form
    │   │   ├── Register.js       # Registration form
    │   │   ├── ProductList.js    # Product grid with search
    │   │   ├── ProductDetail.js  # Product details
    │   │   └── ProductForm.js    # Create/Edit product
    │   ├── contexts/
    │   │   └── AuthContext.js    # Authentication state management
    │   ├── App.js
    │   ├── App.css
    │   └── index.js
    └── package.json
```

---

## 🛠️ Tech Stack

### **Backend**
- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 9.0
- **Authentication**: JWT (JSON Web Tokens)
- **Password Hashing**: SHA256
- **API Documentation**: Swagger/OpenAPI

### **Frontend**
- **Framework**: React 19
- **Routing**: React Router DOM 7
- **HTTP Client**: Axios
- **Styling**: Custom CSS (Cyberpunk theme)
- **State Management**: React Context API

### **Deployment**
- **Backend**: Render (with PostgreSQL database)
- **Frontend**: Vercel
- **CI/CD**: Automatic deployments on git push

---

## 🚀 Getting Started

### **Prerequisites**
- Node.js (v18+ recommended)
- .NET 8.0 SDK
- PostgreSQL (local or cloud)
- Git

### **1. Clone the Repository**
```powershell
git clone https://github.com/ShiroKuro1108/PRN232_Assignment.git
cd PRN232_Assignment/ECommerceApp
```

### **2. Backend Setup**

#### **Install Dependencies**
```powershell
cd ECommerceApp.API
dotnet restore
```

#### **Configure Database**

**Option A: Local PostgreSQL**
Update `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ecommerce_db;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration12345!",
    "Issuer": "ECommerceApp",
    "Audience": "ECommerceApp",
    "ExpiryMinutes": 1440
  }
}
```

**Option B: Render PostgreSQL (Production)**
Set environment variable:
```powershell
$env:DATABASE_URL="postgres://user:password@host:port/database"
```

#### **Run Migrations**
```powershell
dotnet ef database update
```

#### **Run Backend**
```powershell
dotnet run
```
The API will be available at `http://localhost:5000` or `https://localhost:5001`

### **3. Frontend Setup**

#### **Install Dependencies**
```powershell
cd ../client
npm install
```

#### **Configure API URL**
Create `.env` file in `client` folder:
```
REACT_APP_API_URL=http://localhost:5000
```

For production (Vercel):
```
REACT_APP_API_URL=https://your-render-api.onrender.com
```

#### **Run Frontend**
```powershell
npm start
```
The app will open at `http://localhost:3000`

---

## 📡 API Endpoints

### **Authentication**
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login user | No |
| POST | `/api/auth/logout` | Logout user | No |

### **Products**
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/products` | Get all products | No |
| GET | `/api/products/{id}` | Get product by ID | No |
| POST | `/api/products` | Create product | **Yes** |
| PUT | `/api/products/{id}` | Update product | **Yes** |
| DELETE | `/api/products/{id}` | Delete product | **Yes** |

### **Cart**
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/cart` | Get user's cart | **Yes** |
| POST | `/api/cart/items` | Add item to cart | **Yes** |
| PUT | `/api/cart/items/{id}` | Update cart item quantity | **Yes** |
| DELETE | `/api/cart/items/{id}` | Remove item from cart | **Yes** |
| DELETE | `/api/cart/clear` | Clear entire cart | **Yes** |

### **Orders**
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/orders` | Get user's orders | **Yes** |
| GET | `/api/orders/{id}` | Get order by ID | **Yes** |
| POST | `/api/orders` | Create order from cart | **Yes** |
| PUT | `/api/orders/{id}/status` | Update order status | **Yes** |

---

## 🗄️ Database Schema

### **Users**
- `Id` (int, primary key)
- `Email` (string, unique, required)
- `PasswordHash` (string, required)
- `FullName` (string, optional)
- `CreatedAt` (datetime)
- `UpdatedAt` (datetime)

### **Products**
- `Id` (int, primary key)
- `Name` (string, required)
- `Description` (string, required)
- `Price` (decimal, required)
- `ImageUrl` (string, optional)
- `CreatedAt` (datetime)
- `UpdatedAt` (datetime)

### **Carts**
- `Id` (int, primary key)
- `UserId` (int, foreign key)
- `CreatedAt` (datetime)
- `UpdatedAt` (datetime)

### **CartItems**
- `Id` (int, primary key)
- `CartId` (int, foreign key)
- `ProductId` (int, foreign key)
- `Quantity` (int, required)
- `CreatedAt` (datetime)

### **Orders**
- `Id` (int, primary key)
- `UserId` (int, foreign key)
- `TotalAmount` (decimal, required)
- `Status` (string: pending/paid/shipped/delivered/cancelled)
- `CreatedAt` (datetime)
- `UpdatedAt` (datetime)

### **OrderItems**
- `Id` (int, primary key)
- `OrderId` (int, foreign key)
- `ProductId` (int, foreign key)
- `ProductName` (string, snapshot)
- `Price` (decimal, snapshot)
- `Quantity` (int, required)

---

## 🌐 Deployment

### **Backend (Render)**
1. Create PostgreSQL database on Render
2. Create Web Service on Render
3. Connect to GitHub repository
4. Set environment variables:
   - `DATABASE_URL` (auto-set by Render)
   - `JwtSettings__SecretKey`
   - `JwtSettings__Issuer`
   - `JwtSettings__Audience`
5. Deploy automatically on push to main branch

### **Frontend (Vercel)**
1. Connect GitHub repository to Vercel
2. Set environment variable:
   - `REACT_APP_API_URL=https://your-api.onrender.com`
3. Deploy automatically on push to main branch

---

## 🎨 UI Screenshots

*(Add screenshots here after deployment)*

- Homepage with product grid
- Login/Register pages
- Product detail page
- Shopping cart
- Checkout page
- Order history

---

## 🧪 Testing

### **Manual Testing Checklist**
- [ ] Register new user
- [ ] Login with credentials
- [ ] Browse products without login
- [ ] Add product to cart (requires login)
- [ ] Update cart item quantity
- [ ] Remove cart item
- [ ] Complete checkout and place order
- [ ] View order history
- [ ] Create new product (requires login)
- [ ] Edit existing product (requires login)
- [ ] Delete product (requires login)
- [ ] Logout

### **Test Credentials** (for demo)
```
Email: test@example.com
Password: Test123!
```



## 👤 Author

**Dung Quang**
- GitHub: [@ShiroKuro1108](https://github.com/ShiroKuro1108)
- Repository: [PRN232_Assignment](https://github.com/ShiroKuro1108/PRN232_Assignment)

