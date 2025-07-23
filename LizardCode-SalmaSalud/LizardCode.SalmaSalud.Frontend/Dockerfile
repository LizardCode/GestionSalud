# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy framework dependencies first for better caching
COPY ["LizardCode-Framework/", "LizardCode-Framework/"]

# Copy solution file and project files
COPY ["LizardCode-SalmaSalud/LizardCode.SalmaSalud.sln", "LizardCode-SalmaSalud/"]
COPY ["LizardCode-SalmaSalud/LizardCode.SalmaSalud.Frontend/LizardCode.SalmaSalud.csproj", "LizardCode-SalmaSalud/LizardCode.SalmaSalud.Frontend/"]
COPY ["LizardCode-SalmaSalud/LizardCode.SalmaSalud.Application/LizardCode.SalmaSalud.Application.csproj", "LizardCode-SalmaSalud/LizardCode.SalmaSalud.Application/"]
COPY ["LizardCode-SalmaSalud/LizardCode.SalmaSalud.Domain/LizardCode.SalmaSalud.Domain.csproj", "LizardCode-SalmaSalud/LizardCode.SalmaSalud.Domain/"]
COPY ["LizardCode-SalmaSalud/LizardCode.SalmaSalud.Infrastructure/LizardCode.SalmaSalud.Infrastructure.csproj", "LizardCode-SalmaSalud/LizardCode.SalmaSalud.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "LizardCode-SalmaSalud/LizardCode.SalmaSalud.Frontend/LizardCode.SalmaSalud.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/LizardCode-SalmaSalud/LizardCode.SalmaSalud.Frontend"
RUN dotnet build "LizardCode.SalmaSalud.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "LizardCode.SalmaSalud.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080


# Entry point
ENTRYPOINT ["dotnet", "LizardCode.SalmaSalud.dll"]