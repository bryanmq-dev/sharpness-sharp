using sharpness_sharp.Data.Dtos;

namespace sharpness_sharp.Services
{
    public class ProductoService(HttpClient http)
    {
        public async Task<List<ProductoDto>> ObtenerProductos() =>
            await http.GetFromJsonAsync<List<ProductoDto>>("api/productos") ?? new();

        public async Task<List<CategoriaDto>> ObtenerCategorias() =>
            await http.GetFromJsonAsync<List<CategoriaDto>>("api/categorias") ?? new();
    }
}
