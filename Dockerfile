# syntax=docker/dockerfile:1

##############################
# Stage 1: build & publish
##############################
FROM mcr.microsoft.com/dotnet/sdk:8.0-noble AS build
WORKDIR /src

# Copy only the project file first so `dotnet restore` is cached
# as its own layer and only reruns when dependencies change.
COPY HelloApi.csproj .
RUN dotnet restore HelloApi.csproj

# Now copy the rest of the source and publish.
COPY . .
RUN dotnet publish HelloApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

##############################
# Stage 2: runtime (minimal, golden base image)
##############################
# The "chiseled" ASP.NET image is Microsoft's official minimal runtime:
# no shell, no package manager, no extra OS packages, and it already
# runs as the built-in non-root "app" user - smaller attack surface
# and image size than the regular aspnet runtime image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0

COPY --from=build /app/publish .

# Chiseled images run as the non-root "app" user by default already,
# but keep this explicit for clarity/defense-in-depth.
USER app

EXPOSE 8080

ENTRYPOINT ["dotnet", "HelloApi.dll"]
