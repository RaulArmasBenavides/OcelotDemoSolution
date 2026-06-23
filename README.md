# OcelotDemoSolution

Microservicios API Gateway con paginación avanzada, búsqueda y ordenamiento dinámico.

**Original repo:** https://github.com/Chuksken/OcelotDemoSolution.git

---

## 📋 Tabla de Contenidos

- [Arquitectura](#arquitectura)
- [Microservicios](#microservicios)
- [Paginación, Búsqueda y Ordenamiento](#paginación-búsqueda-y-ordenamiento)
- [Ejemplos de Uso](#ejemplos-de-uso)
- [Respuesta API](#respuesta-api)

---

## 🏗️ Arquitectura

```
┌─────────────────────────────────────┐
│       API Gateway (Ocelot)          │
└────────────┬────────────┬───────────┘
             │            │
    ┌────────▼──────┐  ┌──▼──────────┐
    │  CustomerAPI  │  │  ProductAPI │
    │  (SQL Server) │  │ (SQL Server)│
    └───────────────┘  └─────────────┘
             │
    ┌────────▼──────────┐
    │  OrderAPI         │
    │  (MongoDB)        │
    └───────────────────┘
```

---

## 🔧 Microservicios

### 1. **CustomerWebApi** (SQL Server + EF Core)
- Gestión de clientes
- Endpoint: `POST /api/customer`

### 2. **ProductWebApi** (SQL Server + EF Core)
- Gestión de productos
- Endpoint: `GET /api/product`

### 3. **OrderWebApi** (MongoDB)
- Gestión de órdenes
- Endpoint: `GET /api/order`

### 4. **AuthenticationWebApi**
- Autenticación JWT
- Endpoint: `POST /api/account/login`

### 5. **ApiGateway** (Ocelot)
- Enrutador centralizado
- Redirecciona requests a microservicios

---

## 🔍 Paginación, Búsqueda y Ordenamiento

Todos los endpoints GET incluyen soporte para:

### **Parámetros de Query**

| Parámetro | Tipo | Descripción | Ejemplo |
|-----------|------|-------------|---------|
| `pageNumber` | int | Número de página (default: 1) | 1, 2, 3... |
| `pageSize` | int | Registros por página (default: 10, max: 100) | 10, 25, 50 |
| `search` | string | Búsqueda de texto | "John", "producto123" |
| `searchFields` | string | Campos a buscar (comma-separated) | "Name,Email" |
| `sortBy` | string | Campo a ordenar (default: Id) | "Name", "Price" |
| `sortOrder` | string | Orden: "asc" o "desc" (default: asc) | "asc", "desc" |

### **Filtros por Entidad**

#### **Customer** (Filtros adicionales)
```
- name: filtrar por nombre
- email: filtrar por email
- phone: filtrar por teléfono
```

#### **Product** (Filtros adicionales)
```
- name: filtrar por nombre
- category: filtrar por categoría
- priceMin: precio mínimo
- priceMax: precio máximo
```

#### **Order** (Filtros adicionales)
```
- customerId: ID del cliente
- status: estado de la orden
- dateFrom: fecha desde
- dateTo: fecha hasta
```

---

## 📝 Ejemplos de Uso

### **1. Customer - Búsqueda simple con paginación**
```
GET /api/customer?pageNumber=1&pageSize=20&search=John&searchFields=Name,Email&sortBy=Name&sortOrder=asc
```

**Parámetros:**
- Página 1, 20 registros por página
- Busca "John" en campos Name y Email
- Ordena por Name ascendente

---

### **2. Customer - Filtros específicos**
```
GET /api/customer?pageNumber=1&pageSize=10&name=John&email=@gmail.com
```

**Parámetros:**
- Filtra por nombre contiene "John"
- Filtra por email contiene "@gmail.com"
- Paginación default (10 registros)

---

### **3. Product - Búsqueda por rango de precio**
```
GET /api/product?pageNumber=1&pageSize=25&priceMin=10&priceMax=100&sortBy=Price&sortOrder=desc
```

**Parámetros:**
- Productos entre $10 y $100
- Página 1, 25 registros
- Ordena por precio descendente

---

### **4. Product - Búsqueda de texto en múltiples campos**
```
GET /api/product?pageNumber=1&pageSize=20&search=laptop&searchFields=Name,Category&sortBy=Name
```

**Parámetros:**
- Busca "laptop" en Name y Category
- Ordena por Name ascendente

---

### **5. Order - Filtro por rango de fechas**
```
GET /api/order?pageNumber=1&pageSize=50&dateFrom=2024-01-01&dateTo=2024-12-31&status=pending&sortBy=OrderDate&sortOrder=desc
```

**Parámetros:**
- Órdenes entre enero y diciembre de 2024
- Solo órdenes con estado "pending"
- Ordena por fecha descendente

---

### **6. Combinación de búsqueda + filtros + ordenamiento**
```
GET /api/product?pageNumber=2&pageSize=15&search=shoe&searchFields=Name,Category&category=footwear&priceMax=150&sortBy=Price&sortOrder=asc
```

**Parámetros:**
- Búsqueda: "shoe" en Name y Category
- Filtro: categoría "footwear"
- Filtro: precio máximo $150
- Página 2, 15 registros
- Ordena por precio ascendente

---

## 📤 Respuesta API

Todos los endpoints GET devuelven el mismo formato paginado:

### **Ejemplo de respuesta exitosa (200 OK)**

```json
{
  "pageNumber": 1,
  "pageSize": 20,
  "totalRecords": 150,
  "totalPages": 8,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "data": [
    {
      "customerId": 1,
      "name": "John Doe",
      "email": "john@example.com",
      "phone": "555-1234"
    },
    {
      "customerId": 2,
      "name": "Jane Smith",
      "email": "jane@example.com",
      "phone": "555-5678"
    }
  ]
}
```

### **Campos de respuesta**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `pageNumber` | int | Número de página actual |
| `pageSize` | int | Registros por página |
| `totalRecords` | int | Total de registros (sin paginación) |
| `totalPages` | int | Total de páginas disponibles |
| `hasPreviousPage` | bool | ¿Hay página anterior? |
| `hasNextPage` | bool | ¿Hay página siguiente? |
| `data` | array | Registros de la página actual |

---

## 🎯 Casos de Uso Comunes

### **Búsqueda simple**
```bash
curl -X GET "http://localhost:5000/api/customer?pageNumber=1&pageSize=20&search=John&searchFields=Name,Email&sortBy=Name&sortOrder=asc" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Parámetros:**
- Busca "John" en Name y Email
- Página 1, 20 registros
- Ordena por Name ascendente

---

### **Obtener todos los productos caros ordenados de mayor a menor precio**
```bash
curl -X GET "http://localhost:5000/api/product?pageNumber=1&pageSize=50&priceMin=500&sortBy=Price&sortOrder=desc"
```

**Parámetros:**
- Productos con precio >= $500
- Página 1, 50 registros
- Ordena por Price descendente

---

### **Búsqueda de clientes por nombre con email específico**
```bash
curl -X GET "http://localhost:5000/api/customer?pageNumber=1&pageSize=20&search=john&searchFields=Name&email=@company.com&sortBy=Name" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Parámetros:**
- Búsqueda de texto: "john" en Name
- Filtro adicional: email contiene "@company.com"
- Ordena por Name

---

### **Órdenes recientes de un cliente específico**
```bash
curl -X GET "http://localhost:5000/api/order?pageNumber=1&pageSize=20&customerId=123&dateFrom=2024-01-01&sortBy=OrderDate&sortOrder=desc"
```

**Parámetros:**
- Órdenes del cliente con ID 123
- Desde 2024-01-01 hasta hoy
- Página 1, 20 registros
- Ordena por fecha descendente (más recientes primero)

---

### **Productos descuentados en una categoría**
```bash
curl -X GET "http://localhost:5000/api/product?pageNumber=1&pageSize=25&category=Electronics&priceMax=99.99&sortBy=Price&sortOrder=asc"
```

**Parámetros:**
- Categoría: "Electronics"
- Precio máximo: $99.99
- Página 1, 25 registros
- Ordena por Price ascendente (más baratos primero)

---

### **Búsqueda con todos los parámetros combinados**
```bash
curl -X GET "http://localhost:5000/api/product?pageNumber=2&pageSize=15&search=shoe&searchFields=Name,Category&category=footwear&priceMin=20&priceMax=150&sortBy=Price&sortOrder=asc"
```

**Parámetros:**
- Búsqueda: "shoe" en Name y Category
- Filtro: categoría "footwear"
- Filtro: precio entre $20 y $150
- Página 2, 15 registros por página
- Ordena por Price ascendente

---

## 🔒 Validaciones

✅ **pageNumber < 1** → Se ajusta a 1  
✅ **pageSize < 1 o > 100** → Se ajusta a 10  
✅ **sortOrder no es "asc" ni "desc"** → Default "asc"  
✅ **sortBy inválido** → Se ignora y ordena por Id  
✅ **searchFields no existen** → Se ignora la búsqueda  

---

## 📦 Estructura de Carpetas

```
CustomerWebApi/
├── Controllers/
│   └── CustomerController.cs
├── Dtos/
│   ├── Request/
│   │   ├── PagedRequest.cs
│   │   └── CustomerFilterRequest.cs
│   └── Response/
│       ├── PaginatedResponse.cs
│       └── ApiException.cs
├── Extensions/
│   └── PaginationExtensions.cs
├── Models/
│   └── Customer.cs
└── Program.cs

ProductWebApi/ (Similar estructura)
OrderWebApi/ (Similar estructura)
```

---

## 🚀 Instalación

### **Requisitos**
- .NET 6.0+
- SQL Server
- MongoDB (para OrderWebApi)
- Visual Studio o VS Code

### **Pasos**

1. **Clonar repositorio**
```bash
git clone https://github.com/Chuksken/OcelotDemoSolution.git
cd OcelotDemoSolution
```

2. **Configurar base de datos**
```bash
# SQL Server (CustomerWebApi, ProductWebApi)
update-database -Project CustomerWebApi
update-database -Project ProductWebApi
```

3. **Configurar MongoDB**
```bash
# Variables de entorno
set DB_HOST=localhost
set DB_NAME=OrderDb
```

4. **Ejecutar solución**
```bash
dotnet run --project ApiGateway
```

---

## 🔑 Autenticación

Los endpoints de lectura en **CustomerWebApi** requieren JWT token:

```bash
# 1. Obtener token
POST /api/account/login
{
  "userName": "admin",
  "password": "pass123"
}

# 2. Usar token en header
GET /api/customer
Authorization: Bearer <token_aqui>
```

---

## 📊 Performance

### **Recomendaciones**

- ✅ Usar `pageSize` entre 10-50 para mejor rendimiento
- ✅ Evitar offset > 100,000 (usar cursor-based si necesitas)
- ✅ Indexar campos de búsqueda y ordenamiento en BD
- ✅ Limitar `searchFields` a campos esenciales

---

## 🐛 Troubleshooting

### **"404 - Endpoint no encontrado"**
- Verificar que el microservicio está ejecutándose
- Revisar configuración en Ocelot (Program.cs del ApiGateway)

### **"401 - Unauthorized"**
- Verificar que el token JWT es válido
- Renovar token si está expirado

### **"400 - Bad Request"**
- Validar parámetros de query
- Revisar que `sortBy` corresponda a un campo real

---

---

## 🧪 Pruebas Rápidas con cURL

### **1. Sin autenticación (Products)**
```bash
# Todas las paginación default
curl -X GET "http://localhost:5000/api/product"

# Con búsqueda
curl -X GET "http://localhost:5000/api/product?search=laptop&searchFields=Name,Category"

# Con filtros de precio
curl -X GET "http://localhost:5000/api/product?priceMin=10&priceMax=100&sortBy=Price&sortOrder=asc"

# Página específica
curl -X GET "http://localhost:5000/api/product?pageNumber=2&pageSize=25"
```

### **2. Con autenticación (Customers)**
```bash
# Primero obtener token
TOKEN=$(curl -s -X POST "http://localhost:5000/api/account/login" \
  -H "Content-Type: application/json" \
  -d '{"userName":"admin","password":"pass123"}' \
  | jq -r '.data.token')

# Luego usar el token
curl -X GET "http://localhost:5000/api/customer?pageNumber=1&pageSize=20" \
  -H "Authorization: Bearer $TOKEN"

# Con búsqueda y ordenamiento
curl -X GET "http://localhost:5000/api/customer?search=john&searchFields=Name,Email&sortBy=Name&sortOrder=asc" \
  -H "Authorization: Bearer $TOKEN"
```

### **3. MongoDB Orders**
```bash
# Todas las órdenes paginadas
curl -X GET "http://localhost:5000/api/order?pageNumber=1&pageSize=50"

# Órdenes por estado
curl -X GET "http://localhost:5000/api/order?status=completed&sortBy=OrderDate&sortOrder=desc"

# Órdenes en rango de fechas
curl -X GET "http://localhost:5000/api/order?dateFrom=2024-01-01&dateTo=2024-12-31"

# Órdenes de un cliente específico
curl -X GET "http://localhost:5000/api/order?customerId=5&sortBy=OrderDate&sortOrder=desc"
```

### **4. PowerShell**
```powershell
# Get productos con búsqueda (PowerShell)
$uri = "http://localhost:5000/api/product?search=shoe&searchFields=Name&priceMax=100&sortBy=Price&sortOrder=asc"
$response = Invoke-RestMethod -Uri $uri -Method Get
$response | ConvertTo-Json

# Con token
$token = "YOUR_JWT_TOKEN"
$headers = @{"Authorization" = "Bearer $token"}
$uri = "http://localhost:5000/api/customer?pageNumber=1&pageSize=20"
$response = Invoke-RestMethod -Uri $uri -Method Get -Headers $headers
$response.data | Format-Table
```

---

## 📄 Licencia

MIT License - Basado en [OcelotDemoSolution](https://github.com/Chuksken/OcelotDemoSolution)

---

## 👤 Autor

**Raul Armas** - RaulArmasBX@gmail.com
