using sharpness_sharp.Data.Dtos;

namespace sharpness_sharp.Services
{
    public class ProductoService(HttpClient http)
    {
        public async Task<List<ProductoDto>> ObtenerProductos() =>
            await http.GetFromJsonAsync<List<ProductoDto>>("api/productos") ?? new();

        public async Task<List<ProductoDto>> ObtenerProductosVip() =>
            await http.GetFromJsonAsync<List<ProductoDto>>("api/productos/vip") ?? new();

        public async Task<List<CategoriaDto>> ObtenerCategorias() =>
            await http.GetFromJsonAsync<List<CategoriaDto>>("api/categorias") ?? new();

        public async Task<string?> Crear(ProductoGuardarDto dto)
        {
            var respuesta = await http.PostAsJsonAsync("api/productos", dto);
            return await Error(respuesta);
        }

        public async Task<string?> Actualizar(int id, ProductoGuardarDto dto)
        {
            var respuesta = await http.PutAsJsonAsync($"api/productos/{id}", dto);
            return await Error(respuesta);
        }

        public async Task<string?> Eliminar(int id)
        {
            var respuesta = await http.DeleteAsync($"api/productos/{id}");
            return await Error(respuesta);
        }

        private static async Task<string?> Error(HttpResponseMessage respuesta) =>
            respuesta.IsSuccessStatusCode ? null : await respuesta.Content.ReadAsStringAsync();
    }
}
