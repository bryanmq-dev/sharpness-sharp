using Microsoft.EntityFrameworkCore;
using sharpness_sharp.Data;
using sharpness_sharp.Data.Dtos;
using sharpness_sharp.Data.Models;

namespace sharpness_sharp.Repositories
{
    public class VentaRepository(ApplicationAuthDbContext db)
    {
        public async Task<(VentaDto? Venta, string? Error)> Crear(string usuarioId, VentaCrearDto dto)
        {
            if (dto.Items.Count == 0 || dto.Items.Any(item => item.Cantidad <= 0))
                return (null, "El carrito esta vacio o tiene cantidades invalidas.");

            var ids = dto.Items.Select(item => item.ProductoId).Distinct().ToList();
            var productos = await db.Productos.Where(producto => ids.Contains(producto.Id)).ToListAsync();

            if (productos.Count != ids.Count)
                return (null, "Alguno de los productos no existe.");

            var venta = new Venta { UsuarioId = usuarioId };

            foreach (var item in dto.Items)
            {
                var producto = productos.First(p => p.Id == item.ProductoId);

                if (producto.Stock < item.Cantidad)
                    return (null, $"No hay stock suficiente de {producto.Nombre}.");

                producto.Stock -= item.Cantidad;
                venta.Detalles.Add(new DetalleVenta
                {
                    ProductoId = producto.Id,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio
                });
            }

            venta.Total = venta.Detalles.Sum(detalle => detalle.Cantidad * detalle.PrecioUnitario);

            db.Ventas.Add(venta);
            await db.SaveChangesAsync();

            return (await ObtenerVenta(venta.Id), null);
        }

        public async Task<VentaDto?> ObtenerVenta(int id)
        {
            var venta = await db.Ventas
                .AsNoTracking()
                .Include(v => v.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            return venta is null ? null : ADto(venta);
        }

        public async Task<List<VentaDto>> ObtenerDeUsuario(string usuarioId)
        {
            var ventas = await db.Ventas
                .AsNoTracking()
                .Include(v => v.Detalles).ThenInclude(d => d.Producto)
                .Where(v => v.UsuarioId == usuarioId)
                .ToListAsync();

            return ventas.OrderByDescending(v => v.Fecha).Select(ADto).ToList();
        }

        private static VentaDto ADto(Venta venta) =>
            new(venta.Id, venta.Fecha, venta.Total,
                venta.Detalles.Select(d => new VentaDetalleDto(d.Producto.Nombre, d.Cantidad, d.PrecioUnitario)).ToList());
    }
}
