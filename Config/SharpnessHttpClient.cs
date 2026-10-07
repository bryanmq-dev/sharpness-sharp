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
        services.AddHttpClient(ApiClientName, client =>
        {
            var baseUrl = configuration["Api:BaseUrl"];

            if (!string.IsNullOrWhiteSpace(baseUrl))
                client.BaseAddress = new Uri(ConBarraFinal(baseUrl));
        });

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ApiClientName));

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
