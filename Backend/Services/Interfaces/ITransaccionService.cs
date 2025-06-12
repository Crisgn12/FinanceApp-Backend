using Entidades.DTOs;

namespace Backend.Services.Interfaces
{
    public interface ITransaccionService
    {
        Task<bool> IngresarTransaccionAsync(TransaccionDTO transaccionDTO);
        Task<List<TransaccionDTO>> ObtenerTransaccionesPorUsuarioAsync(ObtenerTransaccionesPorUsuarioDTO req);
        Task<TransaccionDTO> ObtenerDetalleTransaccionAsync(ObtenerDetalleTransaccionDTO req);
        Task<bool> ActualizarTransaccionAsync(TransaccionDTO transaccionDTO);
        Task<bool> EliminarTransaccionAsync(EliminarTransaccionDTO req);
        Task<List<TotalxDiaDTO>> TotalGastosUltimos6diasPorUsuarioAsync();
        Task<List<GraficoCategoriasDTO>> TotalGastosPorCategoriaAsync();
    }
}
