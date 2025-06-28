# Use .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY LizardCode-SalmaSalud/*.sln ./
COPY LizardCode-SalmaSalud/LizardCode.SalmaSalud.API/*.csproj ./LizardCode.SalmaSalud.API/
COPY LizardCode-SalmaSalud/LizardCode.SalmaSalud.Application/*.csproj ./LizardCode.SalmaSalud.Application/
COPY LizardCode-SalmaSalud/LizardCode.SalmaSalud.Infrastructure/*.csproj ./LizardCode.SalmaSalud.Infrastructure/
COPY LizardCode-SalmaSalud/LizardCode.SalmaSalud.Domain/*.csproj ./LizardCode.SalmaSalud.Domain/
COPY LizardCode-SalmaSalud/LizardCode.SalmaSalud.Services/*.csproj ./LizardCode.SalmaSalud.Services/
COPY LizardCode-Framework/Helpers/LizardCode.Framework.Helpers.Utilities/*.csproj ./LizardCode-Framework/Helpers/LizardCode.Framework.Helpers.Utilities/

# Restore dependencies
RUN dotnet restore LizardCode.SalmaSalud.API/LizardCode.SalmaSalud.API.csproj

# Copy all source code
COPY LizardCode-SalmaSalud/ ./
COPY LizardCode-Framework/ ./LizardCode-Framework/

# Build the application
RUN dotnet build LizardCode.SalmaSalud.API/LizardCode.SalmaSalud.API.csproj -c Release -o /app/build

# Publish the application
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
