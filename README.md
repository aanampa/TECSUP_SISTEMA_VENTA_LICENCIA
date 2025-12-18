# Sistema de Gestión de Licencias - TECSUP
Sistema modular para la gestión y venta de licencias construido con .NET 8 y arquitectura modular.
## Tecnologías y Herramientas
El proyecto utiliza el siguiente stack tecnológico:
* **Lenguaje:** C#
* **Framework:** NET CORE 8
* **Base de Datos:** SQL Server
* **Herramientas:** Docker & Docker Compose, Visual Studio Code

## Arquitectura del Sistema
```bash
SolucionLicencias/
├── src/
│   ├── SistemaLicencias/              # API Principal (Core Business Logic)
│   ├── SistemaLicencias.Auth/         # Microservicio de Autenticación JWT
│   ├── SistemaLicencias.AdminApp/     # Panel de Administración (Blazor/React)
│   ├── SistemaLicencias.ClienteApp/   # Portal de Clientes
│   ├── SistemaLicencias.Mailing/      # Servicio de Correos (Google/SMTP)
│   └── db/                            # Scripts SQL y migraciones
├── docker-compose.yml                 # Orquestación de contenedores
├── docker-compose.override.yml        # Configuración de desarrollo
└── SolucionLicencias.sln              # Solución Visual Studio 2022
```
## Instalación con Docker

1. Asegúrate de tener Docker instalado.
2. Clonar el repositorio
   ```bash
   git clone https://github.com/aanampa/TECSUP_SISTEMA_VENTA_LICENCIA.git

   # ingresar a carpeta del proyecto
   cd TECSUP_SISTEMA_VENTA_LICENCIA
   ```

4.  Ejecuta el siguiente comando en la raíz del proyecto:
    ```bash
    docker-compose up --build -d
    ```
5.  Verifica que los contenedores estén corriendo:
    ```bash
    docker ps
    ```
6.  Ejecucion:
    ```bash
    Cliente App: http://localhost:9084
    Admin App: http://localhost:9085
    ```
