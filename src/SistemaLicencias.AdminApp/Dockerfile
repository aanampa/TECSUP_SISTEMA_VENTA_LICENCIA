# src/SistemaLicencias.AdminApp/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar csproj
COPY src/SistemaLicencias.AdminApp/SistemaLicencias.AdminApp.csproj src/SistemaLicencias.AdminApp/
RUN dotnet restore "src/SistemaLicencias.AdminApp/SistemaLicencias.AdminApp.csproj"

# Copiar todo y publicar
COPY src/ src/
WORKDIR /app/src/SistemaLicencias.AdminApp
RUN dotnet publish -c Release -o /app/out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "SistemaLicencias.AdminApp.dll"]