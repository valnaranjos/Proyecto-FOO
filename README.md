# Proyecto-FOO

Backend en .NET 8.0 para digitalizar la gestión de intervenciones psicológicas realizadas por el equipo de psicólogas de CAPEP.

🧠 **Objetivo**

El equipo de psicólogas de CAPEP maneja actualmente la gestión de la información en documentos físicos y no cuenta con una herramienta digital que permita registrar, consultar y (a definir) compartir de forma eficiente dicha información sobre las intervenciones realizadas con cada niño/a y su comportamiento antecedente, lo que limita la capacidad de actuar con rapidez y precisión, especialmente durante situaciones de crisis. **Este backend proporciona una API segura para la gestión de esta información.**

🔒 **Seguridad Implementada**

* **Autenticación:** Se ha implementado un sistema de autenticación basado en **JWT (JSON Web Tokens)** para proteger la API. Los usuarios pueden iniciar sesión proporcionando sus credenciales y reciben un token que deben incluir en las solicitudes posteriores para acceder a los recursos protegidos.
* **Autorización:** El **`PatientController`** ahora está protegido y requiere autenticación para acceder a sus endpoints, asegurando que solo los usuarios autenticados puedan gestionar la información de los pacientes.
* **Clave Secreta:** La clave secreta para la firma de los tokens JWT se gestiona a través de **variables de entorno** para mayor seguridad.

🛠️ **Tecnologías**

* .NET 8.0
* EF Core 8.0.408
* MySQL
* Docker
* Swagger para la documentación y prueba de la API.
* **JWT (JSON Web Tokens)** para la autenticación y autorización de usuarios.
* **Microsoft.AspNetCore.Authentication.JwtBearer** para el soporte de JWT en ASP.NET Core.
* CORS (Cross-Origin Resource Sharing) para gestionar las políticas de acceso entre dominios. (AllowAnyOrigin)
* Git Flow para control de versiones.
* EF Core Migrations para la gestión del esquema de la base de datos.
* Estrategias de validación de datos basadas en Data Annotations.

### ⚙️ Configuración

La configuración sensible, como la clave secreta JWT y la cadena de conexión a la base de datos, se gestiona a través de **variables de entorno** para mayor seguridad y flexibilidad entre diferentes entornos (desarrollo, pruebas, producción).

📁 **Estructura del repositorio**
Proyecto-FOO/
│
├── README.md
├── .gitignore
├── ProyectoFoo.sln
│
├── src/
│   ├── ProyectoFoo.API/           # Capa de presentación (controllers, configuración, modelos de request/response)
│   ├── ProyectoFoo.Application/   # Lógica de negocio (casos de uso, interfaces)
│   ├── ProyectoFoo.Domain/        # Entidades de dominio, reglas base y servicios de dominio (ej: ITokenService)
│   ├── ProyectoFoo.Infrastructure/# Acceso a datos (EF Core, repositorios), servicios externos
│   └── ProyectoFoo.Shared/        # Helpers y utilidades compartidas
│
└── tests/
├── ProyectoFoo.Tests.Unit/        # Pruebas unitarias
└── ProyectoFoo.Tests.Integration/ # Pruebas de integración