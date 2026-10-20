using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sharpness_sharp.Data.Models
{
    public class Producto
    {
        public Guid Guid;
        public string UrlImagen = null!;
        public string Nombre = null!;
        public decimal Precio;
        public int Stock;

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