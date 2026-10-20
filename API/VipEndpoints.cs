using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using sharpness_sharp.Config;
using sharpness_sharp.Data;
using sharpness_sharp.Repositories;

namespace sharpness_sharp.API
{
    public class VipEndpoints : IEndpoints
    {
        public const string RolVip = "ClienteVIP";

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/suscripciones", async (ClaimsPrincipal usuario, SuscripcionRepository repositorio) =>
            {
                var usuarioId = usuario.FindFirstValue(ClaimTypes.NameIdentifier)!;

                if (await repositorio.TieneActiva(usuarioId))
                    return Results.BadRequest("Ya tenes una suscripcion activa.");

                return Results.Created("/api/suscripciones", await repositorio.Crear(usuarioId));
            })
            .RequireAuthorization(politica => politica.RequireRole("Cliente"));

            app.MapPut("/api/usuarios/vip", async (ClaimsPrincipal usuario, UserManager<ApplicationUser> usuarios, SuscripcionRepository repositorio) =>
            {
                var usuarioId = usuario.FindFirstValue(ClaimTypes.NameIdentifier)!;

                if (!await repositorio.TieneActiva(usuarioId))
                    return Results.BadRequest("Primero tenes que registrar tu suscripcion.");

                var cliente = await usuarios.FindByIdAsync(usuarioId);
                if (cliente is null) return Results.NotFound();

                if (!await usuarios.IsInRoleAsync(cliente, RolVip))
                    await usuarios.AddToRoleAsync(cliente, RolVip);

                return Results.NoContent();
            })
            .RequireAuthorization(politica => politica.RequireRole("Cliente"));
        }
    }
}
