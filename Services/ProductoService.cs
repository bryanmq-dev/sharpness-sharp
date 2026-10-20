using sharpness_sharp.Data.Dtos;
using sharpness_sharp.Repositories;

namespace sharpness_sharp.Services
{
    public class ProductoService(ProductoRepository repositorio)
    {
        public Task<List<ProductoDto>> ObtenerProductos() => repositorio.ObtenerProductos();

        public Task<List<CategoriaDto>> ObtenerCategorias() => repositorio.ObtenerCategorias();
    }
}
