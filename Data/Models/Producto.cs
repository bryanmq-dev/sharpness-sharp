namespace sharpness_sharp.Data.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string UrlImagen { get; set; } = null!;
        public bool EsVip { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;
    }
}
