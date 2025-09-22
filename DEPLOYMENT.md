# Deployment Guide

This guide covers deploying your e-commerce application to Vercel and Render.

## 🚀 Vercel Deployment

### Prerequisites
- GitHub repository with your code
- Vercel account

### Steps
1. **Push your code to GitHub**
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   git remote add origin your-repo-url
   git push -u origin main
   ```

2. **Deploy to Vercel**
   - Go to [vercel.com](https://vercel.com)
   - Import your GitHub repository
   - Vercel will automatically detect the .NET project
   - Set environment variables in Vercel dashboard:
     - `ConnectionStrings__DefaultConnection`: Your PostgreSQL connection string
     - `ASPNETCORE_ENVIRONMENT`: Production

3. **Database Setup**
   - Use a managed PostgreSQL service like:
     - Vercel Postgres
     - Supabase
     - Railway
     - Neon

### Environment Variables for Vercel
```
ConnectionStrings__DefaultConnection=Host=your-host;Database=your-db;Username=your-user;Password=your-password;SSL Mode=Require
ASPNETCORE_ENVIRONMENT=Production
```

## 🚀 Render Deployment

### Prerequisites
- GitHub repository with your code
- Render account

### Steps
1. **Push your code to GitHub** (same as above)

2. **Create PostgreSQL Database on Render**
   - Go to Render dashboard
   - Create new PostgreSQL database
   - Note the connection details

3. **Deploy Web Service**
   - Create new Web Service on Render
   - Connect your GitHub repository
   - Use these settings:
     - **Build Command**: `dotnet publish -c Release -o out`
     - **Start Command**: `dotnet out/assignment.dll`
     - **Environment**: `dotnet`

4. **Set Environment Variables**
   - `ASPNETCORE_ENVIRONMENT`: Production
   - `ASPNETCORE_URLS`: http://0.0.0.0:$PORT
   - `ConnectionStrings__DefaultConnection`: Your Render PostgreSQL connection string

5. **Run Database Migrations**
   - After first deployment, run migrations via Render shell:
   ```bash
   dotnet ef database update
   ```

### Alternative: Using render.yaml
The included `render.yaml` file can automatically set up both the web service and database.

## 🗄️ Database Options

### Free PostgreSQL Hosting
1. **Render PostgreSQL** (Recommended for Render deployment)
2. **Supabase** (Good for both platforms)
3. **Railway** (Simple setup)
4. **Neon** (Serverless PostgreSQL)
5. **ElephantSQL** (Free tier available)

### Connection String Format
```
Host=your-host;Database=your-db;Username=your-user;Password=your-password;SSL Mode=Require
```

## 🔒 Security Checklist

- [ ] Use environment variables for connection strings
- [ ] Enable SSL for database connections
- [ ] Set proper CORS origins for production
- [ ] Use HTTPS in production
- [ ] Keep sensitive data out of source control

## 🐛 Troubleshooting

### Common Issues
1. **Port binding errors**: Make sure `ASPNETCORE_URLS` is set correctly
2. **Database connection**: Verify connection string format and SSL requirements
3. **Static files not serving**: Ensure `UseStaticFiles()` and `UseDefaultFiles()` are configured
4. **CORS errors**: Update CORS policy for your production domain

### Logs
- Check deployment logs in Vercel/Render dashboard
- Use `dotnet ef database update --verbose` for migration issues

## 📝 Post-Deployment

1. **Test all functionality**:
   - Homepage loads correctly
   - API endpoints work
   - CRUD operations function
   - Search and filtering work

2. **Monitor performance**:
   - Check response times
   - Monitor database connections
   - Watch for errors in logs

3. **Set up custom domain** (optional):
   - Configure DNS settings
   - Update CORS and AllowedHosts settings
