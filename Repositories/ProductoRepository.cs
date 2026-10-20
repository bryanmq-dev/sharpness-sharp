using Microsoft.EntityFrameworkCore;
using sharpness_sharp.Data;
using sharpness_sharp.Data.Dtos;

namespace sharpness_sharp.Repositories
{
    public class ProductoRepository(ApplicationAuthDbContext db)
    {
        public async Task<List<ProductoDto>> ObtenerProductos(int? categoriaId = null)
        {
            var productos = await db.Productos
                .AsNoTracking()
                .Include(producto => producto.Categoria)
                .Where(producto => categoriaId == null || producto.CategoriaId == categoriaId)
                .ToListAsync();

            return productos.Select(producto => producto.ADto()).ToList();
        }

        public async Task<ProductoDto?> ObtenerProducto(int id)
        {
            var producto = await db.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            return producto?.ADto();
        }

        public async Task<List<CategoriaDto>> ObtenerCategorias()
        {
            var categorias = await db.Categorias.AsNoTracking().ToListAsync();

            return categorias.Select(categoria => categoria.ADto()).ToList();
        }
    }
}
