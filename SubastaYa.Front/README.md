# SubastaYa.Front

Frontend de **SubastaYa**: aplicación en React (Vite) + Tailwind CSS que consume la API
REST y se conecta al hub de SignalR para actualizaciones en tiempo real.

> La documentación completa del proyecto (backend, base de datos, endpoints, reglas de
> negocio y detalle del frontend) está en el **README principal** de la raíz:
> [`../README.md`](../README.md).

## Stack

- React 19 + Vite
- Tailwind CSS 4
- React Router (navegación)
- @microsoft/signalr (tiempo real)
- react-hot-toast (notificaciones)

## Requisitos

- Node.js 18+ (probado con Node 24 y npm 12)
- El backend de SubastaYa corriendo (por defecto en `http://localhost:5269`)

## Puesta en marcha

```bash
npm install
npm run dev
```

Queda disponible en `http://localhost:5173`. Ese origen ya está habilitado en la política
CORS de la API.

La URL del backend se configura en `.env`:

```
VITE_API_URL=http://localhost:5269
```

## Scripts

| Comando | Descripción |
|---------|-------------|
| `npm run dev` | Servidor de desarrollo con HMR |
| `npm run build` | Build de producción en `dist/` |
| `npm run preview` | Sirve localmente el build de producción |

## Pantallas (módulos)

- **Catálogo** (`/`): listado de subastas con filtros por estado, categoría y orden;
  contador regresivo por card (hasta el cierre, o hasta el inicio si es próxima).
- **Sala de subasta** (`/auctions/:id`): temporizador, puja actual, indicador de
  liderando/superado, historial de pujas y actualización en tiempo real vía SignalR.
- **Billetera** (`/wallet`): saldos (total, retenido, disponible), carga de saldo e
  historial de movimientos.
- **Publicar** (`/publish`): formulario de creación con validaciones.
- **Mis actividades** (`/my-activity`): "Mis compras/pujas" y "Mis publicaciones".

## Identificación de usuario

No hay autenticación formal. Un selector (arriba a la derecha) permite cambiar entre los
4 usuarios semilla; el elegido se envía en el header `UserId` de cada petición.
