# Dockerfile

# ── Stage 1: Build ──────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything and restore
COPY . .
RUN dotnet restore ProductVault.API/ProductVault.API.csproj

# Publish the API in Release mode
RUN dotnet publish ProductVault.API/ProductVault.API.csproj \
    -c Release \
    -o /app/publish

# ── Stage 2: Run ────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProductVault.API.dll"]