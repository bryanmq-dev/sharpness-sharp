using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sharpness_sharp.Config;
using sharpness_sharp.Data;
using sharpness_sharp.Data.Models;

namespace sharpness_sharp.API
{
    public class ProductoService
    {
        private readonly ApplicationAuthDbContext _db;
        private readonly HttpClient _client;

        public ProductoService()
        {

        }
        public ProductoService(ApplicationAuthDbContext db, HttpClient client) { _db = db; _client = client; }

        public async Task<List<Producto>> ObtenerProductos()
        {
            var productos = await _client.GetFromJsonAsync<List<Producto>>("https://raw.githubusercontent.com/bryanmq-dev/sharpness-sharp/refs/heads/course/imp/product.json");


            return productos ?? new List<Producto>();
        }
    }
}