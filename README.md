# Sharpness Sharp

Plantilla de **Blazor Web App sobre .NET 10** para el curso de desarrollo fullstack con C#.

La idea es sacar del medio el cableado repetitivo — base de datos, login, cliente HTTP,
endpoints — para que cada clase empiece escribiendo la funcionalidad y no `Program.cs`.
Todo el arranque queda en cuatro extensiones dentro de `Config/`.

```csharp
// Program.cs
builder.Services.UseHttpClient(builder.Configuration);
builder.Services.UseAuth(builder.Configuration);   // configura la base por dentro
...
app.UseAPIEndpoints();
```

---

## Requisitos

| | |
|---|---|
| .NET SDK | 10.0 |
| Node + pnpm | sólo para Tailwind (el build lo invoca solo) |
| `dotnet-ef` | ya está en `dotnet-tools.json` → `dotnet tool restore` |

```bash
dotnet tool restore
dotnet run
```

La primera compilación corre `pnpm install` y genera `wwwroot/app.css` con Tailwind.
En Linux y macOS además deja un watcher de CSS andando.

---

## Configuración

Todo vive en `appsettings.json`. **Cualquier clave se puede pisar con variables de
entorno** usando `__` como separador — que es como se configura en Docker y en el
servidor, sin tocar el JSON ni recompilar.

```bash
Database__Engine=POSTGRESQL
Database__Password=loquesea
Api__BaseUrl=https://mi-dominio.com/
```

> Las contraseñas **no van al repositorio**. Usá variables de entorno o
> `dotnet user-secrets set "Database:Password" "..."`.

---

## `UseDB()` — base de datos

Registra el `DbContext` resolviendo el motor en tiempo de arranque.

```json
"Database": {
  "Engine": "SQLITE",
  "Source": "Data/app.db"
}
```

Motores soportados y qué campos pide cada uno:

| `Engine` | Paquete | Campos |
|---|---|---|
| `SQLITE` | `Microsoft.EntityFrameworkCore.Sqlite` | `Source` |
| `POSTGRESQL` | `Npgsql.EntityFrameworkCore.PostgreSQL` | `Host`, `Port`, `Name`, `User`, `Password` |
| `SQLSERVER` | `Microsoft.EntityFrameworkCore.SqlServer` | ídem + `TrustServerCertificate` |

Pasar de SQLite a Supabase no toca una línea de C#:

```json
"Database": {
  "Engine": "POSTGRESQL",
  "Host": "db.xxxx.supabase.co",
  "Port": 5432,
  "Name": "postgres",
  "User": "postgres"
}
```

Detalles que importan:

- La cadena se arma con el `ConnectionStringBuilder` de cada driver, no concatenando
  texto, así una contraseña con `;` o `=` no rompe nada.
- Si falta un dato, la app **no arranca** y dice exactamente cuál y dónde ponerlo:
  `UseDB: falta 'Password' para POSTGRESQL. Definilo en appsettings.json como
  "Database:Password" o con la variable de entorno Database__Password.`
- También existe la versión explícita, para mostrar en clase qué está pasando:
  `services.UseDB(new DatabaseParams(DBEngine.POSTGRESQL, host, port, name, user, password))`.

---

## `UseAuth()` — login, registro y roles

Embebe todo el bloque de ASP.NET Core Identity que el template `-au Individual` deja
suelto en `Program.cs`: el `DbContext`, el esquema de cookies, el
`AuthenticationStateProvider`, los scoped del área de Account, el `UserManager` /
`SignInManager` y el emisor de mails.

```csharp
builder.Services.UseAuth(builder.Configuration);
```

**`UseAuth` llama a `UseDB` por dentro** — no hay que llamar a las dos. Lo único que le
pasa es un flag que decide qué contexto se registra:

| | Contexto registrado | Para qué |
|---|---|---|
| `UseAuth(config)` | `ApplicationAuthDbContext` | la app con login |
| `UseDB(config)` | `ApplicationDbContext` | la app sin login |

Son **dos clases pero un solo contexto vivo**: el flag elige cuál. Con login, el contexto
es un `IdentityDbContext`, así que las tablas de la aplicación conviven con las de Identity
en el mismo modelo — y por lo tanto una `Venta` puede tener una navigation property real
hacia `ApplicationUser`, con su clave foránea y su `Include`.

Las tablas propias se declaran ahí:

```csharp
// Data/ApplicationDbContext.cs
public class ApplicationAuthDbContext(DbContextOptions<ApplicationAuthDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Venta> Ventas => Set<Venta>();
}
```

Lo que ya viene andando, sin escribir una línea: registro, confirmación de cuenta, login,
2FA, passkeys, recuperación de contraseña y exportación de datos personales — las páginas
están en `Components/Account/`.

### Seed inicial

```csharp
// Program.cs, despues de builder.Build()
await app.SeedAuth();
```

En cada arranque, y de forma idempotente:

1. aplica las migraciones pendientes;
2. crea los roles de `Auth:Roles` que falten — `AdminRole` siempre entra;
3. siembra el usuario administrador y lo agrega a su rol.

```json
"Auth": {
  "AdminRole": "Admin",
  "Roles": [ "Admin" ],
  "AdminEmail": "",
  "AdminPassword": ""
}
```

En desarrollo, `appsettings.Development.json` ya trae `admin@sharpness.local` /
`Admin123!`, así que un clon limpio se corre y se entra sin configurar nada.

> **La contraseña de produccion no va al repositorio.** Se pasa por `Auth__AdminPassword`
> o `dotnet user-secrets set "Auth:AdminPassword" "..."`. Si `AdminEmail` o `AdminPassword`
> quedan vacios, los roles se crean igual y el usuario se saltea con un warning en el log.

Para las Clases 4 y 5, los roles nuevos se agregan **sin tocar C#**:

```json
"Roles": [ "Admin", "Cliente", "ClienteVIP" ]
```

El admin sembrado queda con `EmailConfirmed = true` a proposito: `UseAuth` deja
`RequireConfirmedAccount` en `true`, y no hay mail que confirmar.

### Migraciones

El contexto de Identity es el dueño de las migraciones, así que **hay que nombrarlo**:

```bash
dotnet ef migrations add AgregarProductos --context ApplicationAuthDbContext
dotnet ef database update --context ApplicationAuthDbContext
```

La migración inicial `CreateIdentitySchema` ya crea las tablas `AspNet*`. No hace falta
aplicarlas a mano antes del primer arranque: `SeedAuth()` lo hace solo.

---

## `UseHttpClient()` — cliente HTTP

```json
"Api": { "BaseUrl": "https://localhost:7249/" }
```

Registra un `HttpClient` con nombre (`APIClient`) y además lo expone como **scoped**, para
inyectarlo directo en servicios y componentes sin pedir `IHttpClientFactory`.

- Si `Api:BaseUrl` está vacío el cliente igual sirve: sólo hay que usar URLs absolutas,
  que es lo que se hace al consumir una API ajena.
- La barra final se normaliza sola. Sin ella, .NET reemplaza el último segmento:
  con base `https://host/api` pedir `"productos"` termina pegándole a `https://host/productos`.
  Por el mismo motivo, **las rutas relativas no empiezan con `/`**.
- `BaseUrl` tiene que coincidir con el `applicationUrl` de `Properties/launchSettings.json`.
  En Docker va a ser otra, por eso es configuración y no una constante.

---

## `UseAPIEndpoints()` — endpoints automáticos

Descubre por reflexión todas las clases que implementan `IEndpoints` y las mapea. No hay
que tocar `Program.cs` para agregar una API nueva: alcanza con crear la clase.

```csharp
// API/CafesEndpoints.cs
public class CafesEndpoints : IEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/cafes", async (ICafeRepository repo) => await repo.ObtenerAsync());
        app.MapPost("/api/cafes", async (CafeDto dto, ICafeRepository repo) => ...);
    }
}
```

Se descubre por **contrato**, no por nombre: si la clase no implementa `IEndpoints` el
compilador avisa, mientras que un filtro por `"...Endpoints"` fallaría en silencio ante
un typo. Nada se mapea si no se llama a `app.UseAPIEndpoints()`.

Los servicios se inyectan **en el handler**, no en el constructor de la clase: Minimal API
los resuelve por request, y así cada endpoint declara exactamente lo que necesita.

---

## Estructura

```
Config/            las cuatro extensiones de arranque
  SharpnessDB.cs            UseDB + DatabaseParams + DBEngine
  SharpnessAuth.cs          UseAuth
  SharpnessHttpClient.cs    UseHttpClient
  SharpnessAPIEndpoints.cs  UseAPIEndpoints + IEndpoints
API/               las clases IEndpoints de la aplicación
Data/              DbContexts, ApplicationUser y migraciones
Components/        Razor: Layout, Pages y el área Account de Identity
Styles/            entrada de Tailwind (tailwind.css) y el watcher
wwwroot/           app.css generado — no se edita a mano
```

---

## Temario del curso

| Clase | Tema | Qué se usa de la plantilla |
|---|---|---|
| 1 | Frontend: componentes, cards, búsqueda, filtros | Razor + Tailwind |
| 2 | Backend: servicios, DI, HttpClient, API, CRUD | `UseHttpClient`, `UseAPIEndpoints` |
| 3 | Datos: PostgreSQL, EF Core, modelos, relaciones, DTOs | `UseDB`, migraciones |
| 4 | Auth: Identity, autorización, roles, API protegida | `UseAuth` |
| 5 | A producción: Docker y feature VIP de punta a punta | todo junto |

---

## Notas

- Tailwind corre desde el `.csproj` (target `TailwindCss`), antes de `AssignTargetPaths`,
  para que `wwwroot/app.css` exista cuando se arma la lista de static assets.
- `*.db` está en `.gitignore`: la base local no se versiona.
- `RequireConfirmedAccount` está en `true`. El template no manda mails de verdad
  (`IdentityNoOpEmailSender`): muestra el link de confirmación en pantalla.
