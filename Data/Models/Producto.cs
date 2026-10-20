namespace sharpness_sharp.Data.Models
{
    public class Producto
    {
        public string UrlImagen = null!;
        public string Nombre = null!;
        public decimal Precio;
        public int Stock;
        public string Categoria = null!;

        public Producto() { }

        public Producto(string url, string name, decimal price, int stock, string categoria)
        {
            UrlImagen = url;
            Nombre = name;
            Precio = price;
            Stock = stock;
            Categoria = categoria;
        }
    }
}
