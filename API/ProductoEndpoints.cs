using sharpness_sharp.Config;
using sharpness_sharp.Repositories;

namespace sharpness_sharp.API
{
    public class ProductoEndpoints : IEndpoints
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var grupo = app.MapGroup("/api/productos");

            grupo.MapGet("/", async (ProductoRepository repositorio, int? categoriaId) =>
                Results.Ok(await repositorio.ObtenerProductos(categoriaId)));

            grupo.MapGet("/{id:int}", async (int id, ProductoRepository repositorio) =>
                await repositorio.ObtenerProducto(id) is { } producto ? Results.Ok(producto) : Results.NotFound());
        }
    }
}
