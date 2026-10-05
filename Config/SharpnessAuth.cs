using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
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
        /// Va encadenado después de UseDB, porque reutiliza el MISMO motor y la misma
        /// cadena de conexión que se configuró ahí:
        ///
        ///     builder.Services.UseDB(builder.Configuration).UseAuth();
        /// </summary>
        public static IServiceCollection UseAuth(this IServiceCollection services)
        {
            var db = services.FirstOrDefault(d => d.ServiceType == typeof(DatabaseParams))?.ImplementationInstance as DatabaseParams
                ?? throw new InvalidOperationException(
                    "UseAuth() necesita que UseDB() se haya llamado antes: builder.Services.UseDB(...).UseAuth();");

            // El contexto de Identity. Es ApplicationAuthDbContext (el IdentityDbContext),
            // no ApplicationDbContext, que queda libre para las tablas de la aplicación.
            services.AddDbContext<ApplicationAuthDbContext>(db.Configurar);

            // Deja el estado de autenticación disponible como parámetro en cascada,
            // que es lo que consumen <AuthorizeView> y [Authorize] en los componentes.
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
                .AddEntityFrameworkStores<ApplicationAuthDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            // El template no manda mails de verdad: muestra el link de confirmación en pantalla.
            services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            return services;
        }
    }
}
