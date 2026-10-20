using Microsoft.EntityFrameworkCore;
using sharpness_sharp.Data;
using sharpness_sharp.Data.Dtos;
using sharpness_sharp.Data.Models;

namespace sharpness_sharp.Repositories
{
    public class SuscripcionRepository(ApplicationAuthDbContext db)
    {
        public Task<bool> TieneActiva(string usuarioId) =>
            db.Suscripciones.AnyAsync(s => s.UsuarioId == usuarioId && s.FechaFin > DateTime.UtcNow);

        public async Task<SuscripcionDto> Crear(string usuarioId)
        {
            var suscripcion = new Suscripcion { UsuarioId = usuarioId };

            db.Suscripciones.Add(suscripcion);
            await db.SaveChangesAsync();

            return new SuscripcionDto(suscripcion.Id, suscripcion.FechaInicio, suscripcion.FechaFin, suscripcion.Monto);
        }
    }
}
