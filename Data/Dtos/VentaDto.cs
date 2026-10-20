namespace sharpness_sharp.Data.Dtos
{
    public record VentaItemDto(int ProductoId, int Cantidad);

    public record VentaCrearDto(List<VentaItemDto> Items, bool CompraVip = false);

    public record VentaDetalleDto(string Producto, int Cantidad, decimal PrecioUnitario);

    public record VentaDto(int Id, DateTime Fecha, decimal Total, bool CompraVip, List<VentaDetalleDto> Detalles);
}
