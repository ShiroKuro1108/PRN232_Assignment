# Split Deployment Guide: Frontend on Vercel + API on Render

This guide shows how to deploy your frontend to Vercel and API to Render separately.

## 🎯 Architecture Overview

- **Frontend (Vercel)**: Static HTML/CSS/JS files from `wwwroot` folder
- **API (Render)**: .NET Web API with database connection
- **Database (Supabase)**: PostgreSQL database

## 🚀 Step 1: Deploy API to Render

### 1.1 Create Render Account
1. Go to [render.com](https://render.com)
2. Sign up with GitHub

### 1.2 Deploy API
1. Click "New +" → "Web Service"
2. Connect your GitHub repository
3. Configure:
   - **Name**: `ecommerce-api` (or your preferred name)
   - **Environment**: `Docker` or `Native Environment`
   - **Build Command**: `dotnet publish -c Release -o out`
   - **Start Command**: `dotnet out/assignment.dll`

### 1.3 Set Environment Variables
Add these in Render dashboard:
- `ASPNETCORE_ENVIRONMENT` = `Production`
- `ASPNETCORE_URLS` = `http://0.0.0.0:$PORT`
- `ConnectionStrings__DefaultConnection` = Your Supabase connection string

### 1.4 Get API URL
After deployment, copy your API URL (e.g., `https://ecommerce-api-abc123.onrender.com`)

## 🌐 Step 2: Deploy Frontend to Vercel

### 2.1 Update API URL
1. Open `wwwroot/app.js`
2. Replace `your-api-app.onrender.com` with your actual Render API URL:
   ```javascript
   const API_BASE_URL = window.location.hostname === 'localhost' 
       ? '/api/products'
       : 'https://your-actual-api-url.onrender.com/api/products';
   ```

### 2.2 Deploy to Vercel
1. Go to [vercel.com](https://vercel.com)
2. Click "New Project"
3. Import your GitHub repository
4. Vercel will detect the static files automatically
5. Deploy!

## 🔧 Step 3: Configure CORS

Update your .NET API to allow requests from Vercel:

```csharp
// In Program.cs, update CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://your-vercel-app.vercel.app")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Use the policy
app.UseCors("AllowFrontend");
```

## 🗄️ Step 4: Database Migrations

Run migrations against your Supabase database:
```bash
dotnet ef database update --connection "your-supabase-connection-string"
```

## ✅ Testing Your Deployment

1. **API Test**: Visit `https://your-api.onrender.com/api/products`
2. **Frontend Test**: Visit your Vercel URL
3. **Integration Test**: Try creating/editing products from the frontend

## 🔒 Security Checklist

- [ ] Update CORS to allow only your Vercel domain
- [ ] Use environment variables for all secrets
- [ ] Enable HTTPS for all services
- [ ] Test all CRUD operations

## 🐛 Common Issues

1. **CORS Errors**: Update the CORS policy in Program.cs
2. **API Not Found**: Check the API URL in app.js
3. **Database Connection**: Verify Supabase connection string
4. **Build Failures**: Check Render build logs

## 📝 URLs to Save

- **Frontend**: `https://your-app.vercel.app`
- **API**: `https://your-api.onrender.com`
- **Database**: Your Supabase dashboard URL
