namespace Compras.Service.Application.Dtos;

public record CompraResponse(
    int IdCompraCab,
    DateTime FecRegistro,
    decimal SubTotal,
    decimal Igv,
    decimal Total,
    IReadOnlyList<CompraDetalleResponse> Detalles);

public record CompraDetalleResponse(
    int IdCompraDet,
    int IdProducto,
    int Cantidad,
    decimal Precio,
    decimal SubTotal,
    decimal Igv,
    decimal Total);
