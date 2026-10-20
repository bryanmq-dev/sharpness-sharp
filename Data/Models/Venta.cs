namespace sharpness_sharp.Data.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string UsuarioId { get; set; } = null!;
        public ApplicationUser Usuario { get; set; } = null!;
        public decimal Total { get; set; }
        public bool CompraVip { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
    }
}
