using sharpness_sharp.Data.Dtos;

namespace sharpness_sharp.Services
{
    public class CarritoState
    {
        public List<ItemCarrito> Items { get; } = new();

        public event Action? OnCambio;

        public int Cantidad => Items.Sum(item => item.Cantidad);

        public decimal Total => Items.Sum(item => item.Cantidad * item.Producto.Precio);

        public void Agregar(ProductoDto producto)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);

            if (item is null)
                Items.Add(new ItemCarrito(producto, 1));
            else if (item.Cantidad < producto.Stock)
                item.Cantidad++;

            OnCambio?.Invoke();
        }

        public void Quitar(int productoId)
        {
            Items.RemoveAll(item => item.Producto.Id == productoId);
            OnCambio?.Invoke();
        }

        public void Vaciar()
        {
            Items.Clear();
            OnCambio?.Invoke();
        }
    }

    public class ItemCarrito(ProductoDto producto, int cantidad)
    {
        public ProductoDto Producto { get; } = producto;
        public int Cantidad { get; set; } = cantidad;
    }
}
