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

            grupo.MapGet("/", async (ProductoRepository repositorio, int? categoriaId) =>
                Results.Ok(await repositorio.ObtenerProductos(categoriaId)));

            grupo.MapGet("/{id:int}", async (int id, ProductoRepository repositorio) =>
                await repositorio.ObtenerProducto(id) is { } producto ? Results.Ok(producto) : Results.NotFound());

            grupo.MapPost("/", async (ProductoGuardarDto dto, ProductoRepository repositorio) =>
            {
                if (await Validar(dto, repositorio) is { } error) return error;

                var creado = await repositorio.Crear(dto);
                return Results.Created($"/api/productos/{creado.Id}", creado);
            });

            grupo.MapPut("/{id:int}", async (int id, ProductoGuardarDto dto, ProductoRepository repositorio) =>
            {
                if (await Validar(dto, repositorio) is { } error) return error;

                return await repositorio.Actualizar(id, dto) is { } actualizado
                    ? Results.Ok(actualizado)
                    : Results.NotFound();
            });

            grupo.MapDelete("/{id:int}", async (int id, ProductoRepository repositorio) =>
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
