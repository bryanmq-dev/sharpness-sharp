namespace sharpness_sharp.Config;

public static class SharpnessHttpClient
{
    /// <summary>
    /// Configura el HttpClient de la aplicación.
    ///
    /// La dirección base sale de "Api:BaseUrl" (appsettings.json o la variable de entorno
    /// Api__BaseUrl). Si no está definida, el cliente igual funciona: sólo que vas a tener
    /// que pasarle URLs absolutas, que es lo que hacés cuando consumís una API ajena.
    /// </summary>
    public static IServiceCollection UseHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CookieState>();

        services.AddScoped(sp =>
        {
            var handler = new SocketsHttpHandler
            {
                UseCookies = true,
                CookieContainer = sp.GetRequiredService<CookieState>().Contenedor,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            };

            return new HttpClient(handler) { BaseAddress = BaseUri(configuration) };
        });


        return services;
    }

    /// <summary>
    /// BaseAddress TIENE que terminar en "/". Sin la barra, .NET trata el último segmento
    /// como un archivo y lo reemplaza: con base "https://host/api" pedir "productos"
    /// termina pegándole a "https://host/productos".
    /// Por el mismo motivo las rutas relativas NO deben empezar con "/".
    /// </summary>
    private static string ConBarraFinal(string url) => url.EndsWith('/') ? url : url + "/";

    public static Uri? BaseUri(IConfiguration configuration)
    {
        var baseUrl = configuration["Api:BaseUrl"];

        return string.IsNullOrWhiteSpace(baseUrl) ? null : new Uri(ConBarraFinal(baseUrl));
    }
}
