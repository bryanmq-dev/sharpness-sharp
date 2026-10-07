using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using sharpness_sharp.Data;

namespace sharpness_sharp.Config
{
    public static class SharpnessDB
    {
        /// <summary>
        /// Registra el DbContext leyendo la sección "Database" de appsettings.json.
        /// Las variables de entorno la pisan sin tocar código: Database__Engine, Database__Password, etc.
        /// </summary>
        public static IServiceCollection UseDB(this IServiceCollection services, IConfiguration configuration, bool conAuth = false)
            => services.UseDB(DatabaseParams.FromConfiguration(configuration), conAuth);

        /// <summary>
        /// Registra el DbContext con parámetros explícitos.
        /// Preferí la sobrecarga que lee IConfiguration, así la contraseña no queda escrita
        /// en Program.cs.
        /// </summary>
        /// <remarks>
        /// La cadena se arma antes de registrar nada para que un dato faltante falle al
        /// arrancar, con el nombre de la clave, en vez de fallar más tarde con un error
        /// del driver que no dice qué falta.
        /// </remarks>
        /// <param name="conAuth">
        /// Lo manda UseAuth(), y decide CUÁL de los dos contextos se registra. No se usan los dos
        /// a la vez: la aplicación tiene uno solo, y el flag elige cuál.
        ///   false → ApplicationDbContext      (DbContext pelado, para la plantilla sin login)
        ///   true  → ApplicationAuthDbContext  (IdentityDbContext: Identity + las tablas de la app)
        /// </param>
        public static IServiceCollection UseDB(this IServiceCollection services, DatabaseParams db, bool conAuth = false)
        {
            db.ToConnectionString();

            services.AddSingleton(db);

            if (conAuth)
                services.AddDbContext<ApplicationAuthDbContext>(db.Configurar);
            else
                services.AddDbContext<ApplicationDbContext>(db.Configurar);

            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }
    }

    public class DatabaseParams
    {
        public const string SectionName = "Database";

        public DBEngine Engine { get; set; }

        /// <summary>SQLite: ruta del archivo .db.</summary>
        public string? Source { get; set; }

        /// <summary>PostgreSQL / SQL Server.</summary>
        public string? Host { get; set; }
        public int? Port { get; set; }
        public string? Name { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }

        /// <summary>
        /// SQL Server cifra la conexión por defecto y rechaza certificados autofirmados.
        /// En un SQL Server local vas a necesitar esto en true; en producción, NO.
        /// </summary>
        public bool TrustServerCertificate { get; set; }

        public DatabaseParams() { }

        /// <summary>SQLite: sólo hace falta dónde vive el archivo.</summary>
        public DatabaseParams(DBEngine engine, string source)
        {
            Engine = engine;
            Source = source;
        }

        /// <summary>PostgreSQL / SQL Server.</summary>
        public DatabaseParams(DBEngine engine, string host, int port, string name, string user, string password)
        {
            Engine = engine;
            Host = host;
            Port = port;
            Name = name;
            User = user;
            Password = password;
        }

        public static DatabaseParams FromConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(SectionName);

            if (!section.Exists())
                throw new InvalidOperationException(
                    $"UseDB: no existe la sección \"{SectionName}\" en appsettings.json.");

            return section.Get<DatabaseParams>()
                ?? throw new InvalidOperationException(
                    $"UseDB: no se pudo leer la sección \"{SectionName}\".");
        }

        /// <summary>Aplica el proveedor de EF Core que corresponda al motor configurado.</summary>
        public void Configurar(DbContextOptionsBuilder options)
        {
            var connectionString = ToConnectionString();

            switch (Engine)
            {
                case DBEngine.SQLITE:
                    options.UseSqlite(connectionString);
                    break;
                case DBEngine.POSTGRESQL:
                    options.UseNpgsql(connectionString);
                    break;
                case DBEngine.SQLSERVER:
                    options.UseSqlServer(connectionString);
                    break;
                default:
                    throw new InvalidOperationException($"UseDB: motor no soportado ({Engine}).");
            }
        }

        /// <summary>
        /// Arma la cadena con el builder de cada driver en vez de concatenar strings:
        /// así una contraseña con ';' o '=' no rompe la conexión.
        /// </summary>
        public string ToConnectionString() => Engine switch
        {
            DBEngine.SQLITE => new SqliteConnectionStringBuilder
            {
                DataSource = Requerido(Source, nameof(Source)),
                Cache = SqliteCacheMode.Shared,
            }.ToString(),

            DBEngine.POSTGRESQL => new NpgsqlConnectionStringBuilder
            {
                Host = Requerido(Host, nameof(Host)),
                Port = Port ?? 5432,
                Database = Requerido(Name, nameof(Name)),
                Username = Requerido(User, nameof(User)),
                Password = Requerido(Password, nameof(Password)),
            }.ToString(),

            DBEngine.SQLSERVER => new SqlConnectionStringBuilder
            {
                DataSource = Port is null
                    ? Requerido(Host, nameof(Host))
                    : $"{Requerido(Host, nameof(Host))},{Port}",
                InitialCatalog = Requerido(Name, nameof(Name)),
                UserID = Requerido(User, nameof(User)),
                Password = Requerido(Password, nameof(Password)),
                TrustServerCertificate = TrustServerCertificate,
            }.ToString(),

            _ => throw new InvalidOperationException($"UseDB: motor no soportado ({Engine})."),
        };

        private string Requerido(string? valor, string campo) =>
            string.IsNullOrWhiteSpace(valor)
                ? throw new InvalidOperationException(
                    $"UseDB: falta '{campo}' para {Engine}. Definilo en appsettings.json como " +
                    $"\"{SectionName}:{campo}\" o con la variable de entorno {SectionName}__{campo}.")
                : valor;
    }

    public enum DBEngine
    {
        POSTGRESQL,
        SQLSERVER,
        SQLITE
    }
}
