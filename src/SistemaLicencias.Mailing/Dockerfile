# Etapa 1: Construcción (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 1. Copiamos el csproj respetando la estructura
COPY src/SistemaLicencias.Mailing/SistemaLicencias.Mailing.csproj src/SistemaLicencias.Mailing/

# 2. Restauramos dependencias
RUN dotnet restore "src/SistemaLicencias.Mailing/SistemaLicencias.Mailing.csproj"

# 3. Copiamos todo el código fuente
COPY src/ src/

# 4. Publicamos
WORKDIR /app/src/SistemaLicencias.Mailing
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# 5. Copiamos los binarios publicados
# NOTA: Esto incluirá la carpeta "Resources" automáticamente 
# siempre y cuando hayas modificado el .csproj como vimos antes.
COPY --from=build /app/out .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "SistemaLicencias.Mailing.dll"]