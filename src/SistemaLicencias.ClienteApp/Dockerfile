# src/SistemaLicencias.ClienteApp/Dockerfile

# Etapa 1: Construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar csproj y restaurar
COPY src/SistemaLicencias.ClienteApp/SistemaLicencias.ClienteApp.csproj src/SistemaLicencias.ClienteApp/
RUN dotnet restore "src/SistemaLicencias.ClienteApp/SistemaLicencias.ClienteApp.csproj"

# Copiar todo el código y publicar
COPY src/ src/
WORKDIR /app/src/SistemaLicencias.ClienteApp
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "SistemaLicencias.ClienteApp.dll"]