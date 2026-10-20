using sharpness_sharp.Data.Models;

namespace sharpness_sharp.Data.Dtos
{
    public record ProductoDto(int Id, string Nombre, decimal Precio, int Stock, string UrlImagen, string Categoria);

    public record ProductoGuardarDto(string Nombre, decimal Precio, int Stock, string UrlImagen, int CategoriaId);

    public record CategoriaDto(int Id, string Nombre);

    public static class Mapeos
    {
        public static ProductoDto ADto(this Producto producto) =>
            new(producto.Id, producto.Nombre, producto.Precio, producto.Stock, producto.UrlImagen, producto.Categoria.Nombre);

        public static CategoriaDto ADto(this Categoria categoria) =>
            new(categoria.Id, categoria.Nombre);
    }
}
