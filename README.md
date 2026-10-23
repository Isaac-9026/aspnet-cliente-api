# ClienteApi - ASP.NET Core Web API

API RESTful desarrollada con **ASP.NET Core (.NET 10)** y **Entity Framework Core**, diseñada para la gestión y administración de **Clientes**, **Productos** y **Usuarios**. Cuenta con persistencia en **Microsoft SQL Server**, autenticación y autorización mediante **JWT (JSON Web Tokens)** con encriptación **BCrypt**, y documentación interactiva mediante **Scalar API Reference**.

---

## Tecnologías Utilizadas

- **Framework**: [.NET 10 (ASP.NET Core Web API)](https://dotnet.microsoft.com/)
- **Lenguaje**: C#
- **ORM**: Entity Framework Core 10 (`Microsoft.EntityFrameworkCore.SqlServer`)
- **Seguridad**: JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) y Hashing con `BCrypt.Net-Next`
- **Base de Datos**: Microsoft SQL Server
- **Documentación OpenAPI**: [Scalar](https://scalar.com/) (`Scalar.AspNetCore`)

---

## Base de Datos (SQL Server)

El proyecto utiliza la base de datos `DemoClientes`. A continuación se detalla el script SQL para crear la base de datos, las tablas con sus respectivas restricciones e insertar datos de prueba:

```sql
CREATE DATABASE DemoClientes
GO

USE DemoClientes
GO

-- 1. Tabla de Clientes
CREATE TABLE clientes
(
    id          INT IDENTITY(1,1) PRIMARY KEY,
    apellidos   NVARCHAR(70)    NOT NULL,
    nombres     NVARCHAR(70)    NOT NULL,
    telefono    NVARCHAR(9)     NOT NULL,
    correo      NVARCHAR(150)   NOT NULL,
    dni         CHAR(8)         NOT NULL,
    direccion   NVARCHAR(150)   NOT NULL,
    CONSTRAINT uk_dni_cli UNIQUE (dni),
    CONSTRAINT ck_dni_cli CHECK (dni LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
)
GO

-- Datos iniciales de Clientes
INSERT INTO clientes (apellidos, nombres, telefono, correo, dni, direccion) VALUES
('Torres Peña', 'Maria', '987654321', 'maria@gmail.com', '11221122', 'Pueblo Nuevo'),
('Ramos Perez', 'Jose', '907251326', 'jose@gmail.com', '22331112', 'Chincha alta')
GO

-- 2. Tabla de Productos
CREATE TABLE productos
(
    id          INT IDENTITY(1,1) PRIMARY KEY,
    nombre      NVARCHAR(100)   NOT NULL,
    descripcion NVARCHAR(100)   NOT NULL,
    precio      DECIMAL(7,2)    NOT NULL,
    garantia    TINYINT         NOT NULL,
    numserie    NVARCHAR(40)    NULL
)
GO

-- Datos iniciales de Productos
INSERT INTO productos (nombre, descripcion, precio, garantia, numserie) VALUES
('Teclado', 'Logitech negro USB', 75.00, 12, 'ABC-12345'),
('Mouse', 'Genius USB', 40.00, 6, NULL)
GO

-- 3. Tabla de Usuarios (Autenticación)
CREATE TABLE Usuario
(
    id           INT IDENTITY(1,1) PRIMARY KEY,
    username     NVARCHAR(50)     NOT NULL,
    passwordhash NVARCHAR(200)    NOT NULL,
    rol          NVARCHAR(30)     NOT NULL DEFAULT 'Usuario'
)
GO
```

---

## Configuración del Proyecto

### 1. Cadena de Conexión y JWT
En el archivo [`appsettings.json`](file:///c:/Users/SENATI/source/repos/ClienteApi/ClienteApi/appsettings.json), configura tus credenciales de SQL Server y los parámetros de firma JWT:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DemoClientes;User Id=sa;Password=tu_password;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "IngresaUnaClaveDeNoMenos32CaracteresParaPROTEGERWEBJWT:2026",
    "Issuer": "ClientesApi",
    "Audience": "ClientesApiUsuarios",
    "ExpireMinutes": 60
  }
}
```

---

## Endpoints de la API

### Módulo Autenticación (`/api/auth`)

| Método | Endpoint | Descripción | Requiere Token |
| :--- | :--- | :--- | :--- |
| **POST** | `/api/auth/registrar` | Registra un nuevo usuario con contraseña cifrada (BCrypt) | No |
| **POST** | `/api/auth/login` | Autentica las credenciales y genera el token JWT | No |

---

### Módulo Clientes (`/api/clientes`)

*Nota: Todas las rutas requieren token de autenticación (`[Authorize]`). La ruta de eliminación requiere además rol `Admin`.*

| Método | Endpoint | Descripción | Autorización |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/clientes` | Obtiene el listado completo de clientes | Bearer Token |
| **GET** | `/api/clientes/{id}` | Busca un cliente por su ID | Bearer Token |
| **POST** | `/api/clientes` | Registra un nuevo cliente (valida DNI único) | Bearer Token |
| **PUT** | `/api/clientes/{id}` | Actualiza la información de un cliente existente | Bearer Token |
| **DELETE** | `/api/clientes/{id}` | Elimina un cliente por su ID | Rol: Admin |

---

### Módulo Productos (`/api/productos`)

| Método | Endpoint | Descripción | Código de Estado |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/productos` | Obtiene el listado completo de productos | `200 OK` |
| **GET** | `/api/productos/{id}` | Busca un producto por su ID | `200 OK` / `404 Not Found` |
| **POST** | `/api/productos` | Registra un nuevo producto | `201 Created` |
| **PUT** | `/api/productos/{id}` | Actualiza un producto existente | `204 NoContent` / `400` / `404` |
| **DELETE** | `/api/productos/{id}` | Elimina un producto por su ID | `204 NoContent` / `404 Not Found` |

---

## Pruebas de Funcionamiento (Test)

A continuación se detallan las pruebas de verificación realizadas sobre la API:

### 1. Intento de acceso sin autenticación
Petición a un endpoint protegido sin enviar cabecera de autorización:
- **Método**: `GET`
- **URL**: `https://localhost:7281/api/clientes`
- **Resultado**: `401 Unauthorized` (Sin autorización)

---

### 2. Registro de Usuario
Creación de un nuevo usuario en la base de datos:
- **Método**: `POST`
- **URL**: `https://localhost:7281/api/auth/registrar`
- **Body (JSON)**:
  ```json
  {
    "username": "jhon",
    "password": "clave123"
  }
  ```
- **Resultado**: `200 OK`
  ```json
  {
    "mensaje": "Usuario registrado correctamente"
  }
  ```

---

### 3. Inicio de Sesión (Login) y Obtención del Token JWT
Autenticación del usuario y recepción del token:
- **Método**: `POST`
- **URL**: `https://localhost:7281/api/auth/login`
- **Body (JSON)**:
  ```json
  {
    "username": "jhon",
    "password": "clave123"
  }
  ```
- **Resultado**: `200 OK`
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expira": "2026-10-23T15:10:01.8988164Z"
  }
  ```

---

### 4. Consulta Autorizada con Bearer Token
Petición incluyendo el token en las cabeceras de autorización (`Authorization: Bearer <token>`):
- **Método**: `GET`
- **URL**: `https://localhost:7281/api/clientes`
- **Header**: `Authorization: Bearer eyJhbGciOiJIUzI1Ni...`
- **Resultado**: `200 OK`
  ```json
  [
    {
      "id": 1,
      "apellidos": "Torres Peña",
      "nombres": "María",
      "telefono": "956111222",
      "correo": "maria@gmail.com",
      "dni": "11221122",
      "direccion": "Pueblo Nuevo"
    }
  ]
  ```

---

### 5. Autorización con Rol de Administrador
Para probar la restricción de eliminación (`[Authorize(Roles = "Admin")]`), se actualiza el rol del usuario a `Admin` directamente en SQL Server:

```sql
UPDATE Usuario SET rol = 'Admin' WHERE id = 2
GO
```

Al iniciar sesión con dicho usuario y generar un token con privilegios de administrador, se procede con la eliminación:
- **Método**: `DELETE`
- **URL**: `https://localhost:7281/api/clientes/1`
- **Header**: `Authorization: Bearer <token_admin>`
- **Resultado**: `204 NoContent` (Eliminación exitosa)

---

## Documentación Interactiva (Scalar)

Una vez ejecutada la aplicación en entorno de desarrollo (`Development`), puedes acceder a la interfaz de documentación interactiva en:

- **`https://localhost:7281/scalar/v1`**
- **`http://localhost:5273/scalar/v1`**

Permite autorizarse ingresando el token en el botón **Authorize / Bearer** para probar endpoints protegidos directamente desde el navegador.

---

## Cómo Ejecutar la API Localmente

### Prerrequisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Microsoft SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) instalado y en ejecución

### Pasos
1. Clona el repositorio:
   ```bash
   git clone https://github.com/Isaac-9026/aspnet-cliente-api.git
   cd aspnet-cliente-api
   ```
2. Ejecuta el script SQL en SQL Server Management Studio (SSMS) o Azure Data Studio.
3. Restaura y compila el proyecto:
   ```bash
   dotnet build
   ```
4. Inicia la aplicación:
   ```bash
   dotnet run --project ClienteApi/ClienteApi.csproj
   ```

---