# Multi-stage build for ASP.NET Core 6.0 application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY Fundo.Applications.WebApi/*.csproj ./Fundo.Applications.WebApi/
COPY Fundo.Core/*.csproj ./Fundo.Core/
COPY Fundo.DAL/*.csproj ./Fundo.DAL/
COPY Fundo.Services.Tests/*.csproj ./Fundo.Services.Tests/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . ./

# Build and publish the application
WORKDIR /src/Fundo.Applications.WebApi
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose port 80 (Render will map this to the PORT environment variable)
EXPOSE 80

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Run the application
# Note: Render.com will inject PORT environment variable
# ASP.NET Core will use the PORT variable if ASPNETCORE_URLS is set in environment
ENTRYPOINT ["dotnet", "Fundo.Applications.WebApi.dll"]
