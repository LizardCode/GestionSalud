# Use .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all source code first
COPY . ./

# Set working directory to the solution folder
WORKDIR /src/LizardCode-SalmaSalud

# Create a temporary fixed solution file with correct paths
RUN sed 's|\.\.\\LizardCode-Framework\\|..\\LizardCode-Framework\\|g' LizardCode.SalmaSalud.sln > LizardCode.SalmaSalud.Fixed.sln

# Try to restore using the API project directly (safer approach)
RUN dotnet restore LizardCode.SalmaSalud.API/LizardCode.SalmaSalud.API.csproj

# Build the API project
RUN dotnet build LizardCode.SalmaSalud.API/LizardCode.SalmaSalud.API.csproj -c Release -o /app/build

# Publish the API project
RUN dotnet publish LizardCode.SalmaSalud.API/LizardCode.SalmaSalud.API.csproj -c Release -o /app/publish

# Use .NET 8 runtime for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Copy published application
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the application
ENTRYPOINT ["dotnet", "LizardCode.SalmaSalud.API.dll"]
