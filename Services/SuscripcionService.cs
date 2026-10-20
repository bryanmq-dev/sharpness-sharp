namespace sharpness_sharp.Services
{
    public class SuscripcionService(HttpClient http)
    {
        public async Task<string?> Suscribir()
        {
            var suscripcion = await http.PostAsync("api/suscripciones", null);
            if (!suscripcion.IsSuccessStatusCode) return await suscripcion.Content.ReadAsStringAsync();

            var rol = await http.PutAsync("api/usuarios/vip", null);
            return rol.IsSuccessStatusCode ? null : await rol.Content.ReadAsStringAsync();
        }
    }
}
