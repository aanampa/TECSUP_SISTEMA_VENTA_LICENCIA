# Etapa 1: Construcción (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 1. Copiamos el csproj respetando la estructura de carpetas desde la raíz
COPY src/SistemaLicencias.Auth/SistemaLicencias.Auth.csproj src/SistemaLicencias.Auth/

# 2. Restauramos dependencias
RUN dotnet restore "src/SistemaLicencias.Auth/SistemaLicencias.Auth.csproj"

# 3. Copiamos todo el código fuente (src completo)
COPY src/ src/

# 4. Publicamos la aplicación en la carpeta /app/out
WORKDIR /app/src/SistemaLicencias.Auth
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# 5. Copiamos los archivos publicados desde la etapa de construcción
COPY --from=build /app/out .

# Información de puertos (Kestrel usará los que definiste en appsettings.json)
EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "SistemaLicencias.Auth.dll"]