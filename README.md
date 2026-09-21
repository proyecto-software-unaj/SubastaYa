# SubastaYa

Plataforma de subastas en tiempo real con billetera virtual, escrow (saldo en garantía),
anti-sniping y cierre automático de subastas. El proyecto se compone de un backend REST
en C# / .NET 10 con Entity Framework Core (Code-First) y SQL Server, y un frontend que
consume la API (en desarrollo).

## Enlaces del proyecto

- **Repositorio**: https://github.com/proyecto-software-unaj/SubastaYa
- **Tablero de tareas (Google Sheets)**: https://docs.google.com/spreadsheets/d/1ceW4pjqkTWj_E7ePzfELcVtn6rCQSqGb7saVBbJph6o/edit?usp=sharing

## Estructura del repositorio

```
SubastaYa/
├── SubastaYa.Api/          Backend (.NET 10, EF Core, SQL Server)
│   ├── Domain/             Entidades y enums del negocio
│   ├── Application/        Contratos (interfaces), DTOs y patrón Result
│   ├── Infrastructure/     DbContext, Fluent API, migraciones, seed y servicios
│   └── SubastaYa.Api/      API REST, SignalR Hub, worker de cierre y Swagger
└── SubastaYa.Front/        Frontend (React + Vite + Tailwind)
```

## Arquitectura del backend

Solución en capas con responsabilidades separadas:

- **Domain**: entidades y enums como POCOs, sin dependencias de infraestructura.
- **Application**: interfaces de servicios, DTOs de request/response y el patrón `Result`
  para traducir errores de negocio a códigos HTTP.
- **Infrastructure**: `AppDbContext`, configuraciones Fluent API, migraciones Code-First,
  seed de datos y la implementación de la lógica (pujas, billetera, cierre, auditoría).
- **SubastaYa.Api**: controllers REST, Hub de SignalR, background worker y Swagger UI.

## Requisitos

- .NET SDK 10
- SQL Server 2022 (o SQL Server Express)
- Herramienta EF Core: `dotnet tool install --global dotnet-ef`

## Configuración de la base de datos

La cadena de conexión está en `SubastaYa.Api/SubastaYa.Api/appsettings.json`.
Ajustá el `Server` a tu instancia local de SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_INSTANCIA;Database=SubastaYa;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

## Levantar el backend

Desde la carpeta que contiene la solución (`SubastaYa.Api/`):

```bash
# Restaurar y compilar
dotnet build SubastaYa.Api/SubastaYa.Api.slnx

# Aplicar migraciones (crea la base y las tablas)
dotnet ef database update --project Infrastructure --startup-project SubastaYa.Api

# Ejecutar la API
dotnet run --project SubastaYa.Api
```

Al arrancar, la aplicación aplica automáticamente las migraciones pendientes y siembra
los datos de prueba.

- **API base**: `http://localhost:5269`
- **Swagger UI**: `http://localhost:5269/swagger`
- **Hub SignalR**: `http://localhost:5269/hubs/auctions`

## Datos semilla (usuarios de prueba)

El usuario se identifica mediante el header `UserId` (sin autenticación formal en esta entrega).

| UserId | Email | Total | Retenido | Disponible |
|--------|-------|-------|----------|------------|
| 1 | vendedor@test.com | variable* | 0 | variable* |
| 2 | comprador1@test.com | 150.000 | 45.000 | 105.000 |
| 3 | comprador2@test.com | 200.000 | 8.500 | 191.500 |
| 4 | sinfondos@test.com | 500 | 0 | 500 |

\* Los saldos del vendedor varían según las subastas que el worker haya cerrado y liquidado.

Se siembran además 4 categorías y 5 subastas que cubren los casos de prueba:
activa estándar, activa crítica (cierra en < 2 min), próxima (+24 h), vencida con ganador
y vencida desierta.

## Endpoints principales

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | /api/auctions | Listado con filtros (status, categoryId, minPrice, maxPrice, sort) y paginación |
| POST | /api/auctions | Crear subasta |
| GET | /api/auctions/{id} | Detalle de subasta |
| GET | /api/auctions/{id}/bids | Historial de pujas |
| POST | /api/auctions/{id}/bids | Registrar puja (evalúa saldo, incremento, anti-sniping y concurrencia) |
| GET | /api/wallet/balance | Desglose de saldos (total, retenido, disponible) |
| POST | /api/wallet/deposit | Acreditar saldo simulado |
| GET | /api/categories | Listado de categorías |

Códigos HTTP utilizados: 200 (ok), 201 (creado), 400 (validación), 404 (no existe),
409 (conflicto de concurrencia o de estado) y 422 (saldo insuficiente).

## Reglas de negocio destacadas

- **Escrow atómico**: al pujar se retiene el saldo del nuevo líder y se libera el del
  anterior, en una única transacción.
- **Anti-sniping**: una puja válida dentro de los últimos 60 segundos extiende el cierre
  2 minutos adicionales.
- **Cierre automático**: un worker en segundo plano finaliza las subastas vencidas
  (liquidación comprador → vendedor) o las marca desiertas si no tuvieron pujas.
- **Optimistic locking**: columna `RowVersion` en `Auction` y `Wallet`; los conflictos de
  concurrencia devuelven 409.
- **Auditoría**: cambios de estado, extensiones anti-sniping, rechazos por concurrencia y
  acreditaciones manuales quedan registrados en la tabla de auditoría.

## Prueba de concurrencia (Stress Test)

El script `SubastaYa.Api/SubastaYa.Api/Test/stress-test.ps1` lanza 5 pujas idénticas en
paralelo sobre la misma subasta. Demuestra que la base acepta **una sola** (200) y rechaza
el resto (409), gracias al optimistic locking sobre `RowVersion`.

```powershell
# Con la API corriendo, en otra terminal:
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\SubastaYa.Api\SubastaYa.Api\Test\stress-test.ps1
```

Resultado esperado:

```
200
409
409
409
409
```

Solo una puja queda registrada en la tabla `Bids`; el conflicto de concurrencia se audita
como `BidRejectedConcurrency`.

## Coleccion Postman

En `SubastaYa.Api/SubastaYa.Api/Test/SubastaYa.postman_collection.json` se incluye una
coleccion lista para importar en Postman (menu **Import**). Cubre los casos principales de
la API: catalogo, detalle, historial de pujas, consulta de billetera, puja valida (200),
puja con monto bajo (400), puja sin fondos (422), deposito y creacion de subasta (201).

La coleccion define la variable `baseUrl` en `http://localhost:5269` y usa el header
`UserId` para identificar al usuario en cada peticion. El caso de conflicto de concurrencia
(409) se demuestra con el script de stress test descrito arriba, no desde un request
individual.

## Frontend

Aplicación en React (Vite) + Tailwind CSS que consume la API de forma asíncrona.
Vive en `SubastaYa.Front/`.

### Stack

- React 19 + Vite
- Tailwind CSS 4
- React Router (navegación)
- @microsoft/signalr (tiempo real)
- react-hot-toast (notificaciones)

### Requisitos

- Node.js 18+ (probado con Node 24 y npm 12)

### Levantar el frontend

Desde `SubastaYa.Front/`:

```bash
npm install
npm run dev
```

Queda disponible en `http://localhost:5173` y requiere que el backend esté corriendo
(ese origen ya está habilitado en la política CORS de la API).

La URL del backend se configura en `SubastaYa.Front/.env`:

```
VITE_API_URL=http://localhost:5269
```

### Identificación de usuario

No hay autenticación formal. El frontend incluye un selector de usuario (arriba a la
derecha) que permite cambiar entre los 4 usuarios semilla. El usuario elegido se envía en
el header `UserId` de cada petición. Esto facilita probar el escrow: pujar con un usuario,
cambiar a otro y observar la liberación/retención de saldo.

### Pantallas (módulos)

- **Catálogo** (`/`): listado de subastas con filtros por estado, categoría y orden.
  Cada card muestra un contador regresivo (tiempo hasta el cierre, o hasta el inicio si la
  subasta es próxima) que cambia de color en el último minuto.
- **Sala de subasta** (`/auctions/:id`): temporizador, puja actual, indicador de
  liderando/superado, consola de puja con sugerencia automática del próximo valor,
  historial de pujas y actualización en tiempo real vía SignalR.
- **Billetera** (`/wallet`): panel de saldos (total, retenido, disponible), carga de saldo
  e historial de movimientos.
- **Publicar** (`/publish`): formulario de creación con validaciones (fechas coherentes,
  inicio no en el pasado, precios positivos).
- **Mis actividades** (`/my-activity`): pestañas de "Mis compras/pujas" y "Mis publicaciones".

### Tiempo real

La sala de subasta se conecta al hub SignalR (`/hubs/auctions`) y se une al grupo de la
subasta. Cuando otro usuario puja o se dispara la extensión anti-sniping, la vista se
actualiza sola, sin recargar.

## Mejoras futuras

- Autenticación real con login y JWT (reemplazaría el selector de usuario y el header `UserId`).
- Carga de imágenes propias en la publicación (subida de archivos con sugerencia 400x300)
  e imágenes semilla servidas localmente.
- Historial de movimientos con paginación y filtros.
