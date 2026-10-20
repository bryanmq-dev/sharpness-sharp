using sharpness_sharp.Data.Dtos;

namespace sharpness_sharp.Services
{
    public class VentaService(HttpClient http)
    {
        public async Task<string?> Crear(VentaCrearDto dto)
        {
            var respuesta = await http.PostAsJsonAsync("api/ventas", dto);
            return respuesta.IsSuccessStatusCode ? null : await respuesta.Content.ReadAsStringAsync();
        }

        public async Task<List<VentaDto>> ObtenerMias() =>
            await http.GetFromJsonAsync<List<VentaDto>>("api/ventas/mias") ?? new();
    }
}
