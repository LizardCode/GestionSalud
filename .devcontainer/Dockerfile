# GestionSalud Development Container - Minimal .NET 8 Environment
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy

# Set environment variables for development
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
ENV DOTNET_NOLOGO=true

# Install minimal essential packages
RUN apt-get update \
    && apt-get -y install --no-install-recommends \
        git \
        curl \
        ca-certificates \
    && apt-get autoremove -y \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*

# Create the vscode user
ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID

RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME

# Create workspace directory
RUN mkdir -p /workspace \
    && chown -R $USERNAME:$USERNAME /workspace

# Switch to the vscode user
USER $USERNAME

# Install essential .NET global tools
RUN dotnet tool install --global dotnet-ef

# Generate HTTPS development certificate
RUN dotnet dev-certs https --trust

# Set the working directory
WORKDIR /workspace