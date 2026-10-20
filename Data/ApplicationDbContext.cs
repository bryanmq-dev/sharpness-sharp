using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sharpness_sharp.Data.Models;

namespace sharpness_sharp.Data;

public class ApplicationAuthDbContext(DbContextOptions<ApplicationAuthDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Suscripcion> Suscripciones => Set<Suscripcion>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Categoria>()
            .HasMany(categoria => categoria.Productos)
            .WithOne(producto => producto.Categoria)
            .HasForeignKey(producto => producto.CategoriaId);

        builder.Entity<Producto>()
            .Property(producto => producto.Precio)
            .HasPrecision(10, 2);

        builder.Entity<Suscripcion>().Property(suscripcion => suscripcion.Monto).HasPrecision(10, 2);

        builder.Entity<Venta>()
            .HasMany(venta => venta.Detalles)
            .WithOne(detalle => detalle.Venta)
            .HasForeignKey(detalle => detalle.VentaId);

        builder.Entity<Venta>().Property(venta => venta.Total).HasPrecision(10, 2);
        builder.Entity<DetalleVenta>().Property(detalle => detalle.PrecioUnitario).HasPrecision(10, 2);

        builder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Cafe" },
            new Categoria { Id = 2, Nombre = "Bebidas" },
            new Categoria { Id = 3, Nombre = "Panaderia" },
            new Categoria { Id = 4, Nombre = "Postres" });

        builder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Americano", Precio = 12m, Stock = 100, UrlImagen = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd", CategoriaId = 1 },
            new Producto { Id = 2, Nombre = "Capuccino", Precio = 16m, Stock = 90, UrlImagen = "https://images.unsplash.com/photo-1572442388796-11668a67e53d", CategoriaId = 1 },
            new Producto { Id = 3, Nombre = "Latte", Precio = 18m, Stock = 80, UrlImagen = "https://images.unsplash.com/photo-1541167760496-1628856ab772", CategoriaId = 1 },
            new Producto { Id = 4, Nombre = "Flat White", Precio = 17m, Stock = 60, UrlImagen = "https://images.unsplash.com/photo-1517256064527-09c73fc73e38", CategoriaId = 1 },
            new Producto { Id = 5, Nombre = "Cafe Filtrado", Precio = 15m, Stock = 40, UrlImagen = "https://images.unsplash.com/photo-1442512595331-e89e73853f31", CategoriaId = 1 },
            new Producto { Id = 6, Nombre = "Cafe Helado", Precio = 18m, Stock = 50, UrlImagen = "https://images.unsplash.com/photo-1461023058943-07fcbe16d735", CategoriaId = 2 },
            new Producto { Id = 7, Nombre = "Te Helado", Precio = 14m, Stock = 45, UrlImagen = "https://images.unsplash.com/photo-1556679343-c7306c1976bc", CategoriaId = 2 },
            new Producto { Id = 8, Nombre = "Te con Leche", Precio = 13m, Stock = 35, UrlImagen = "https://images.unsplash.com/photo-1544787219-7f47ccb76574", CategoriaId = 2 },
            new Producto { Id = 9, Nombre = "Croissant", Precio = 12m, Stock = 60, UrlImagen = "https://images.unsplash.com/photo-1555507036-ab1f4038808a", CategoriaId = 3 },
            new Producto { Id = 10, Nombre = "Donas", Precio = 15m, Stock = 80, UrlImagen = "https://images.unsplash.com/photo-1551024506-0bccd828d307", CategoriaId = 3 },
            new Producto { Id = 11, Nombre = "Galletas con Chips", Precio = 8m, Stock = 120, UrlImagen = "https://images.unsplash.com/photo-1558961363-fa8fdf82db35", CategoriaId = 3 },
            new Producto { Id = 12, Nombre = "Pastel de Chocolate", Precio = 45m, Stock = 25, UrlImagen = "https://images.unsplash.com/photo-1578985545062-69928b1d9587", CategoriaId = 4 },
            new Producto { Id = 13, Nombre = "Brownie", Precio = 14m, Stock = 55, UrlImagen = "https://images.unsplash.com/photo-1571115177098-24ec42ed204d", CategoriaId = 4 },
            new Producto { Id = 14, Nombre = "Cupcakes", Precio = 18m, Stock = 70, UrlImagen = "https://images.unsplash.com/photo-1495147466023-ac5c588e2e94", CategoriaId = 4 },
            new Producto { Id = 15, Nombre = "Geisha Reserva", Precio = 40m, Stock = 15, UrlImagen = "https://images.unsplash.com/photo-1497935586351-b67a49e012bf", CategoriaId = 1, EsVip = true },
            new Producto { Id = 16, Nombre = "Cata de Cafes", Precio = 55m, Stock = 10, UrlImagen = "https://images.unsplash.com/photo-1509042239860-f550ce710b93", CategoriaId = 1, EsVip = true },
            new Producto { Id = 17, Nombre = "Latte Art Premium", Precio = 30m, Stock = 20, UrlImagen = "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085", CategoriaId = 1, EsVip = true });
    }
}

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
}
