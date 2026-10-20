namespace sharpness_sharp.Services
{
    public class CookieState
    {
        public string? Valor { get; set; }
    }

    public class CookieHandler(CookieState estado, HttpMessageHandler interno) : DelegatingHandler(interno)
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage solicitud, CancellationToken token)
        {
            if (!string.IsNullOrEmpty(estado.Valor))
                solicitud.Headers.TryAddWithoutValidation("Cookie", estado.Valor);

            return base.SendAsync(solicitud, token);
        }
    }
}
