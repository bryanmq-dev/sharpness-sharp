using Microsoft.EntityFrameworkCore;
using sharpness_sharp.Data;
using sharpness_sharp.Data.Dtos;
using sharpness_sharp.Data.Models;

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

        public async Task<bool> ExisteCategoria(int categoriaId) =>
            await db.Categorias.AnyAsync(categoria => categoria.Id == categoriaId);

        public async Task<ProductoDto> Crear(ProductoGuardarDto dto)
        {
            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Stock = dto.Stock,
                UrlImagen = dto.UrlImagen,
                CategoriaId = dto.CategoriaId
            };

            db.Productos.Add(producto);
            await db.SaveChangesAsync();

            return (await ObtenerProducto(producto.Id))!;
        }

        public async Task<ProductoDto?> Actualizar(int id, ProductoGuardarDto dto)
        {
            var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == id);
            if (producto is null) return null;

            producto.Nombre = dto.Nombre;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock;
            producto.UrlImagen = dto.UrlImagen;
            producto.CategoriaId = dto.CategoriaId;
            await db.SaveChangesAsync();

            return await ObtenerProducto(id);
        }

        public async Task<bool> Eliminar(int id)
        {
            var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == id);
            if (producto is null) return false;

            db.Productos.Remove(producto);
            await db.SaveChangesAsync();

            return true;
        }
    }
}
