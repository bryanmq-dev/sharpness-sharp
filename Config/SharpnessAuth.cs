using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sharpness_sharp.Components.Account;
using sharpness_sharp.Data;

namespace sharpness_sharp.Config
{
    public static class SharpnessAuth
    {
        /// <summary>
        /// Todo lo que el template `-au Individual` dejaba suelto en Program.cs:
        /// el DbContext de Identity, el esquema de cookies, el AuthenticationStateProvider,
        /// los scoped del área de Account y el UserManager/SignInManager.
        ///
        /// UseAuth es la entrada: llama a UseDB() por dentro con conAuth: true, así la base
        /// queda configurada una sola vez y con el contexto que corresponde.
        ///
        ///     builder.Services.UseAuth(builder.Configuration);   // con login
        ///     builder.Services.UseDB(builder.Configuration);     // sin login
        /// </summary>
        public static IServiceCollection UseAuth(this IServiceCollection services, IConfiguration configuration)
            => services.UseAuth(DatabaseParams.FromConfiguration(configuration));

        /// <summary>Lo mismo, con los parámetros de la base escritos a mano.</summary>
        /// <remarks>
        /// AddRoles va ANTES de AddEntityFrameworkStores: el store de roles sólo se registra
        /// si el IdentityBuilder ya tiene seteado el tipo de rol. Invertir ese orden deja la
        /// aplicación sin RoleManager y hace que [Authorize(Roles = "...")] nunca autorice,
        /// porque el claim de rol no entra en la cookie.
        /// </remarks>
        public static IServiceCollection UseAuth(this IServiceCollection services, DatabaseParams db)
        {
            services.UseDB(db, conAuth: true);

            services.AddCascadingAuthenticationState();

            services.AddScoped<IdentityRedirectManager>();
            services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationAuthDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            return services;
        }

        /// <summary>
        /// Deja la base lista para usar: aplica las migraciones pendientes, crea los roles
        /// de "Auth:Roles" y siembra el usuario administrador.
        ///
        ///     await app.SeedAuth();
        ///
        /// Es idempotente: correrlo en cada arranque no duplica nada.
        /// </summary>
        /// <remarks>
        /// El administrador se crea con EmailConfirmed en true a propósito: UseAuth deja
        /// RequireConfirmedAccount en true y no hay mail que confirmar, así que sin esa
        /// bandera el usuario sembrado no podría iniciar sesión.
        ///
        /// Migrar al arrancar alcanza para un despliegue de una sola instancia. Con varias
        /// réplicas levantando a la vez conviene sacar el MigrateAsync y aplicar las
        /// migraciones en un paso aparte del deploy.
        /// </remarks>
        public static async Task<WebApplication> SeedAuth(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var log = sp.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(SharpnessAuth));

            var db = sp.GetService<ApplicationAuthDbContext>()
                ?? throw new InvalidOperationException(
                    "SeedAuth() necesita UseAuth(). Si la aplicación va sin login, sacá la llamada de Program.cs.");

            await db.Database.MigrateAsync();

            var auth = AuthParams.FromConfiguration(app.Configuration);
            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var rol in auth.TodosLosRoles())
            {
                if (await roleManager.RoleExistsAsync(rol)) continue;

                var creado = await roleManager.CreateAsync(new IdentityRole(rol));
                if (!creado.Succeeded)
                    throw new InvalidOperationException($"SeedAuth: no se pudo crear el rol '{rol}'. {Errores(creado)}");

                log.LogInformation("SeedAuth: rol '{Rol}' creado.", rol);
            }

            if (string.IsNullOrWhiteSpace(auth.AdminEmail) || string.IsNullOrWhiteSpace(auth.AdminPassword))
            {
                log.LogWarning(
                    "SeedAuth: roles listos, pero no se sembró el administrador porque falta Auth:AdminEmail " +
                    "o Auth:AdminPassword. Definilos con variables de entorno (Auth__AdminPassword) o user-secrets.");
                return app;
            }

            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
            var admin = await userManager.FindByEmailAsync(auth.AdminEmail);

            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = auth.AdminEmail,
                    Email = auth.AdminEmail,
                    EmailConfirmed = true,
                };

                var creado = await userManager.CreateAsync(admin, auth.AdminPassword);
                if (!creado.Succeeded)
                    throw new InvalidOperationException(
                        $"SeedAuth: no se pudo crear el administrador '{auth.AdminEmail}'. {Errores(creado)}");

                log.LogInformation("SeedAuth: administrador '{Email}' creado.", auth.AdminEmail);
            }

            if (!await userManager.IsInRoleAsync(admin, auth.AdminRole))
            {
                var asignado = await userManager.AddToRoleAsync(admin, auth.AdminRole);
                if (!asignado.Succeeded)
                    throw new InvalidOperationException(
                        $"SeedAuth: no se pudo asignar el rol '{auth.AdminRole}'. {Errores(asignado)}");

                log.LogInformation("SeedAuth: '{Email}' agregado al rol '{Rol}'.", auth.AdminEmail, auth.AdminRole);
            }

            return app;
        }

        private static string Errores(IdentityResult resultado) =>
            string.Join(" ", resultado.Errors.Select(e => $"[{e.Code}] {e.Description}"));
    }

    public class AuthParams
    {
        public const string SectionName = "Auth";

        /// <summary>Rol del administrador. Siempre se crea, esté o no en Roles.</summary>
        public string AdminRole { get; set; } = "Admin";

        /// <summary>Roles que se crean al arrancar. Agregar uno nuevo es editar la configuración.</summary>
        public string[] Roles { get; set; } = [];

        /// <summary>Vacíos ⇒ se crean los roles pero no se siembra el usuario.</summary>
        public string? AdminEmail { get; set; }
        public string? AdminPassword { get; set; }

        public IEnumerable<string> TodosLosRoles() =>
            new[] { AdminRole }
                .Concat(Roles)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct(StringComparer.OrdinalIgnoreCase);

        public static AuthParams FromConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(SectionName);

            return section.Exists()
                ? section.Get<AuthParams>() ?? new AuthParams()
                : new AuthParams();
        }
    }
}
