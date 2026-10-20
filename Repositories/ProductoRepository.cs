using Microsoft.EntityFrameworkCore;
using sharpness_sharp.Data;
using sharpness_sharp.Data.Dtos;

namespace sharpness_sharp.Repositories
{
    public class ProductoRepository(ApplicationAuthDbContext db)
    {
        public async Task<List<ProductoDto>> ObtenerProductos()
        {
            var productos = await db.Productos
                .AsNoTracking()
                .Include(producto => producto.Categoria)
                .ToListAsync();

            return productos.Select(producto => producto.ADto()).ToList();
        }

        public async Task<List<CategoriaDto>> ObtenerCategorias()
        {
            var categorias = await db.Categorias.AsNoTracking().ToListAsync();

            return categorias.Select(categoria => categoria.ADto()).ToList();
        }
    }
}
