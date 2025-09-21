# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["Opptell.Api/Opptell.Api.csproj", "Opptell.Api/"]
RUN dotnet restore "Opptell.Api/Opptell.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Opptell.Api"
RUN dotnet build "Opptell.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Opptell.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final

# Install curl for health checks (optional)
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Create a non-privileged user that the app will run under
ARG UID=10001
RUN useradd \
    --uid "${UID}" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --system \
    appuser

WORKDIR /app
COPY --from=publish --chown=appuser:appuser /app/publish .

# Switch to the non-privileged user to run the application
USER appuser

# Use ASPNETCORE_HTTP_PORTS instead of EXPOSE for .NET 8+
ENV ASPNETCORE_HTTP_PORTS=8080

# Add health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Opptell.Api.dll"]