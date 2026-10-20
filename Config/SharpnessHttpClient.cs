using sharpness_sharp.Services;

namespace sharpness_sharp.Config;

public static class SharpnessHttpClient
{
    public const string ApiClientName = "APIClient";

    /// <summary>
    /// Configura el HttpClient de la aplicación.
    ///
    /// La dirección base sale de "Api:BaseUrl" (appsettings.json o la variable de entorno
    /// Api__BaseUrl). Si no está definida, el cliente igual funciona: sólo que vas a tener
    /// que pasarle URLs absolutas, que es lo que hacés cuando consumís una API ajena.
    /// </summary>
    public static IServiceCollection UseHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(ApiClientName);
        services.AddScoped<CookieState>();

        services.AddScoped(sp =>
        {
            var pipeline = sp.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(ApiClientName);
            var cliente = new HttpClient(new CookieHandler(sp.GetRequiredService<CookieState>(), pipeline), disposeHandler: false);

            var baseUrl = configuration["Api:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
                cliente.BaseAddress = new Uri(ConBarraFinal(baseUrl));

            return cliente;
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
}
