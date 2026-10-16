# ClienteApi - ASP.NET Core Web API

API RESTful desarrollada con **ASP.NET Core (.NET 10)** y **Entity Framework Core**, diseñada para la gestión y administración de **Clientes** y **Productos**. Cuenta con persistencia en **Microsoft SQL Server** y documentación interactiva mediante **Scalar API Reference**.

---

## Tecnologías Utilizadas

- **Framework**: [.NET 10 (ASP.NET Core Web API)](https://dotnet.microsoft.com/)
- **Lenguaje**: C#
- **ORM**: Entity Framework Core 10 (`Microsoft.EntityFrameworkCore.SqlServer`)
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
```

---

## Configuración del Proyecto

### 1. Cadena de Conexión
En el archivo [`appsettings.json`](file:///c:/Users/SENATI/source/repos/ClienteApi/ClienteApi/appsettings.json), configura tus credenciales de SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DemoClientes;User Id=sa;Password=tu_password;TrustServerCertificate=True;"
  }
}
```

> **Nota**: Si utilizas autenticación de Windows, puedes usar:
> `"Server=localhost;Database=DemoClientes;Trusted_Connection=True;TrustServerCertificate=True;"`

---

## Endpoints de la API

### Módulo Clientes (`/api/clientes`)

| Método | Endpoint | Descripción | Código de Estado |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/clientes` | Obtiene el listado completo de clientes | `200 OK` |
| **GET** | `/api/clientes/{id}` | Busca un cliente por su ID | `200 OK` / `404 Not Found` |
| **POST** | `/api/clientes` | Registra un nuevo cliente (valida DNI único) | `201 Created` / `409 Conflict` |
| **PUT** | `/api/clientes/{id}` | Actualiza la información de un cliente existente | `204 NoContent` / `400` / `404` / `409` |
| **DELETE** | `/api/clientes/{id}` | Elimina un cliente por su ID | `204 NoContent` / `404 Not Found` |

#### Ejemplo JSON para `POST /api/clientes`:
```json
{
  "apellidos": "Gómez Soto",
  "nombres": "Carlos",
  "telefono": "912345678",
  "correo": "carlos.gomez@gmail.com",
  "dni": "45871236",
  "direccion": "Av. Las Palmeras 123"
}
```

---

### Módulo Productos (`/api/productos`)

| Método | Endpoint | Descripción | Código de Estado |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/productos` | Obtiene el listado completo de productos | `200 OK` |
| **GET** | `/api/productos/{id}` | Busca un producto por su ID | `200 OK` / `404 Not Found` |
| **POST** | `/api/productos` | Registra un nuevo producto | `201 Created` |
| **PUT** | `/api/productos/{id}` | Actualiza un producto existente | `204 NoContent` / `400` / `404` |
| **DELETE** | `/api/productos/{id}` | Elimina un producto por su ID | `204 NoContent` / `404 Not Found` |

#### Ejemplo JSON para `POST /api/productos`:
```json
{
  "nombre": "Monitor Gamer 24\"",
  "descripcion": "144Hz IPS 1ms",
  "precio": 650.00,
  "garantia": 24,
  "numserie": "MON-987654"
}
```

---

## Documentación Interactiva (Scalar)

Una vez ejecutada la aplicación en entorno de desarrollo (`Development`), puedes acceder a la interfaz de documentación interactiva en:

- **`https://localhost:7097/scalar/v1`**  
- **`http://localhost:5273/scalar/v1`**

---

## Cómo Ejecutar la API Localmente

### Prerrequisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Microsoft SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) instalado y en ejecución

### Pasos
1. Clona el repositorio:
   ```bash
   git clone https://github.com/isaac-9026/ClienteApi.git
   cd ClienteApi
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