# Ocelot API Gateway con Consul y Health Checks

Esta demo ahora incluye integración con **Consul** para service discovery y **health checks** para monitoreo de servicios.

## Cambios Implementados

### 1. **Consul Service Discovery**
- Agregado Consul como contenedor en `docker-compose.yml`
- Ocelot configurado para descubrir servicios dinámicamente desde Consul
- Cada microservicio se registra automáticamente al iniciar

### 2. **Health Checks**
- Endpoint `/health` expuesto en cada microservicio
- Health checks incluyen validación de conexión a base de datos (Customer, Product)
- Consul monitorea constantemente la salud de los servicios

### 3. **Service Discovery en Ocelot**
El archivo `ocelot.json` ahora usa:
```json
"ServiceName": "customer-service",
"UseServiceDiscovery": true
```

En lugar de hosts/puertos fijos. Consul resuelve automáticamente dónde están los servicios.

## Cómo Ejecutar

### Con Docker Compose:
```bash
docker-compose up -d
```

Esto levantará:
- **Consul UI**: http://localhost:8500
- **API Gateway**: http://localhost:8001
- **Microservicios**: Registrados automáticamente en Consul

### Verificar Servicios Registrados:

1. Accede a Consul UI: http://localhost:8500/ui/
2. En el tab "Services" verás:
   - `authentication-service`
   - `customer-service`
   - `product-service`
   - `order-service`

### Verificar Health Checks:

```bash
# Desde tu máquina (si los puertos están expuestos):
curl http://localhost:8001/health

# O dentro del contenedor:
docker exec customer-api curl http://localhost/health
```

## Configuración de Servicios

Cada servicio está configurado con:

```csharp
var registration = new AgentServiceRegistration()
{
    ID = "customer-service-" + Environment.MachineName,
    Name = "customer-service",
    Address = "customerwebapi",
    Port = 80,
    Check = new AgentServiceCheck()
    {
        HTTP = "http://customerwebapi/health",
        Interval = TimeSpan.FromSeconds(10),        // Check cada 10s
        Timeout = TimeSpan.FromSeconds(5),          // Timeout de 5s
        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)  // Desregistrar si falla 3 veces
    }
};
```

## Propiedades del Health Check

- **Interval**: Cada 10 segundos, Consul verifica si el servicio está saludable
- **Timeout**: Si no responde en 5 segundos, se considera fallido
- **DeregisterCriticalServiceAfter**: Después de 30 segundos de fallos, Consul desregistra automáticamente el servicio

## Rutas Ejemplo

Ahora todas las rutas usan service discovery:

```bash
# Authentication (sin autenticación)
curl -X POST http://localhost:8001/api/Account \
  -H "Content-Type: application/json" \
  -d '{"Username":"admin","Password":"password"}'

# Customer (sin autenticación)
curl http://localhost:8001/api/Customer

# Product (requiere rol Administrator)
curl -H "Authorization: Bearer <TOKEN>" http://localhost:8001/api/Product

# Order (rate limited a 1 request/60s)
curl http://localhost:8001/api/Order
```

## Próximos Pasos Opcionales

1. **Load Balancing**: Agregar múltiples instancias del mismo servicio
2. **Logging Distribuido**: Integrar Seq o ELK para tracing
3. **Circuit Breaker**: Agregar Polly para resilencia
4. **Metrics**: Integrar Prometheus para monitoreo

## Troubleshooting

### Los servicios no se registran en Consul:
- Verifica que Consul esté corriendo: `docker ps | grep consul`
- Revisa logs: `docker logs api-gateway`
- Consul debe estar en red `backend` del docker-compose

### Health checks fallan:
- Verifica conexión a bases de datos
- Comprueba que los endpoints `/health` existan
- Revisa logs de Consul: `docker logs consul-server`

### Ocelot no encuentra los servicios:
- Verifica `ocelot.json` tiene `"UseServiceDiscovery": true`
- Confirma `ServiceDiscoveryProvider.Host` es `consul` (no localhost)
- Reconstruye los contenedores: `docker-compose up --build`
