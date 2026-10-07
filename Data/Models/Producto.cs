using System.Text.Json.Serialization;

namespace sharpness_sharp.Data.Models
{
    public class Producto
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        [JsonPropertyName("imagen")]
        public string UrlImagen { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public Producto() { }

        public Producto(string url, string name, decimal price, int stock)
        {
            UrlImagen = url;
            Nombre = name;
            Precio = price;
            Stock = stock;
        }
    }
}
