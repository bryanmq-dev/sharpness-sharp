namespace sharpness_sharp.Data.Models
{
    public class Suscripcion
    {
        public const decimal PrecioMensual = 99m;

        public int Id { get; set; }
        public string UsuarioId { get; set; } = null!;
        public ApplicationUser Usuario { get; set; } = null!;
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public DateTime FechaFin { get; set; } = DateTime.UtcNow.AddDays(30);
        public decimal Monto { get; set; } = PrecioMensual;
    }
}
