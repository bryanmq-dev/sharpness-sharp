namespace sharpness_sharp.Data.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public List<Producto> Productos { get; set; } = new();
    }
}
