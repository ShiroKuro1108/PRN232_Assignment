#!/bin/bash

# Deployment script for Render or other platforms

echo "Starting deployment..."

# Restore packages
echo "Restoring NuGet packages..."
dotnet restore

# Build the application
echo "Building application..."
dotnet build -c Release

# Run database migrations
echo "Running database migrations..."
dotnet ef database update --no-build

echo "Deployment completed successfully!"
