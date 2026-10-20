using sharpness_sharp.Config;
using sharpness_sharp.Repositories;

namespace sharpness_sharp.API
{
    public class CategoriaEndpoints : IEndpoints
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/categorias", async (ProductoRepository repositorio) =>
                Results.Ok(await repositorio.ObtenerCategorias()));
        }
    }
}
