using System.Security.Claims;
using sharpness_sharp.Config;
using sharpness_sharp.Data.Dtos;
using sharpness_sharp.Repositories;

namespace sharpness_sharp.API
{
    public class VentaEndpoints : IEndpoints
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var grupo = app.MapGroup("/api/ventas")
                .RequireAuthorization(politica => politica.RequireRole("Cliente"));

            grupo.MapPost("/", async (VentaCrearDto dto, ClaimsPrincipal usuario, VentaRepository repositorio) =>
            {
                var (venta, error) = await repositorio.Crear(usuario.FindFirstValue(ClaimTypes.NameIdentifier)!, dto);

                return venta is null ? Results.BadRequest(error) : Results.Created($"/api/ventas/{venta.Id}", venta);
            });

            grupo.MapGet("/mias", async (ClaimsPrincipal usuario, VentaRepository repositorio) =>
                Results.Ok(await repositorio.ObtenerDeUsuario(usuario.FindFirstValue(ClaimTypes.NameIdentifier)!)));
        }
    }
}
