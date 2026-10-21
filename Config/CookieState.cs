using System.Net;

namespace sharpness_sharp.Config
{
    public class CookieState(IConfiguration configuration)
    {
        public const string PrefijoSesion = ".AspNetCore.Identity.Application";

        public CookieContainer Contenedor { get; } = new();

        public static Dictionary<string, string> DeSesion(HttpRequest solicitud) =>
            solicitud.Cookies
                .Where(cookie => cookie.Key.StartsWith(PrefijoSesion))
                .ToDictionary(cookie => cookie.Key, cookie => cookie.Value);

        public void Cargar(Dictionary<string, string>? cookies)
        {
            var destino = SharpnessHttpClient.BaseUri(configuration);
            if (cookies is null || destino is null) return;

            foreach (var (nombre, valor) in cookies)
                Contenedor.Add(destino, new Cookie(nombre, valor) { HttpOnly = true });
        }
    }
}
