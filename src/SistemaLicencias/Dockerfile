# src/SistemaLicencias.Sistema/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 1. Copiar csproj (Ajusta el nombre si tu carpeta se llama diferente)
COPY src/SistemaLicencias/Web.csproj src/SistemaLicencias/
RUN dotnet restore "src/SistemaLicencias/Web.csproj"

# 2. Copiar todo
COPY src/ src/

# 3. Publicar
WORKDIR /app/src/SistemaLicencias
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Web.dll"]