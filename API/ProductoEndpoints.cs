using System.Security.Claims;
using sharpness_sharp.Config;
using sharpness_sharp.Data.Dtos;
using sharpness_sharp.Repositories;

namespace sharpness_sharp.API
{
    public class ProductoEndpoints : IEndpoints
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var grupo = app.MapGroup("/api/productos");

            grupo.MapGet("/", async (ProductoRepository repositorio, ClaimsPrincipal usuario, int? categoriaId) =>
                Results.Ok(await repositorio.ObtenerProductos(categoriaId, usuario.IsInRole("Admin"))));

            grupo.MapGet("/vip", async (ProductoRepository repositorio) =>
                Results.Ok(await repositorio.ObtenerProductosVip()))
                .RequireAuthorization(politica => politica.RequireRole(VipEndpoints.RolVip));

            grupo.MapGet("/{id:int}", async (int id, ProductoRepository repositorio, ClaimsPrincipal usuario) =>
            {
                var producto = await repositorio.ObtenerProducto(id);

                if (producto is null) return Results.NotFound();
                if (producto.EsVip && !usuario.IsInRole(VipEndpoints.RolVip) && !usuario.IsInRole("Admin")) return Results.Forbid();

                return Results.Ok(producto);
            });

            var escritura = grupo.MapGroup("").RequireAuthorization(politica => politica.RequireRole("Admin"));

            escritura.MapPost("/", async (ProductoGuardarDto dto, ProductoRepository repositorio) =>
            {
                if (await Validar(dto, repositorio) is { } error) return error;

                var creado = await repositorio.Crear(dto);
                return Results.Created($"/api/productos/{creado.Id}", creado);
            });

            escritura.MapPut("/{id:int}", async (int id, ProductoGuardarDto dto, ProductoRepository repositorio) =>
            {
                if (await Validar(dto, repositorio) is { } error) return error;

                return await repositorio.Actualizar(id, dto) is { } actualizado
                    ? Results.Ok(actualizado)
                    : Results.NotFound();
            });

            escritura.MapDelete("/{id:int}", async (int id, ProductoRepository repositorio) =>
                await repositorio.Eliminar(id) ? Results.NoContent() : Results.NotFound());
        }

        private static async Task<IResult?> Validar(ProductoGuardarDto dto, ProductoRepository repositorio)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre)) return Results.BadRequest("El nombre es obligatorio.");
            if (dto.Precio <= 0) return Results.BadRequest("El precio debe ser mayor a cero.");
            if (dto.Stock < 0) return Results.BadRequest("El stock no puede ser negativo.");
            if (!await repositorio.ExisteCategoria(dto.CategoriaId)) return Results.BadRequest("La categoria no existe.");

            return null;
        }
    }
}
