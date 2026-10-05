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
        /// UseAuth es la entrada: llama a UseDB() por dentro con conAuth: true, así la base
        /// queda configurada una sola vez y con el contexto que corresponde.
        ///
        ///     builder.Services.UseAuth(builder.Configuration);   // con login
        ///     builder.Services.UseDB(builder.Configuration);     // sin login
        /// </summary>
        public static IServiceCollection UseAuth(this IServiceCollection services, IConfiguration configuration)
            => services.UseAuth(DatabaseParams.FromConfiguration(configuration));

        /// <summary>Lo mismo, con los parámetros de la base escritos a mano.</summary>
        public static IServiceCollection UseAuth(this IServiceCollection services, DatabaseParams db)
        {
            // El flag es lo único que cambia en UseDB: registra ApplicationAuthDbContext
            // en lugar de ApplicationDbContext. Un solo contexto, no dos.
            services.UseDB(db, conAuth: true);

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
