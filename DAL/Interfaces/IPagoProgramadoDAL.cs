using Entidades.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IPagoProgramadoDAL
    {
        Task<bool> CrearPagoProgramadoAsync(CrearPagoProgramadoDTO dto);
        Task<bool> ActualizarPagoProgramadoAsync(PagoProgramadoDTO dto);
        Task<bool> EliminarPagoProgramadoAsync(PagoProgramadoDTO dto);
        Task<bool> CambiarEstadoPagoProgramadoAsync(CambioEstadoDTO dto);
        Task<List<PagoProgramadoDTO>> ObtenerPagosProgramadosAsync(FiltroPagosProgramadosDTO filtro);
        Task<List<PagoProgramadoDTO>> ObtenerPagosProximosAsync(FiltroPagosProximosDTO filtro);
    }
}