using Entidades.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IPagoProgramadoDAL
    {
        Task<bool> CrearPagoProgramadoAsync(CrearPagoProgramadoDTO dto);
        Task<bool> ActualizarPagoProgramadoAsync(PagoProgramadoDTO dto);
        Task<bool> EliminarPagoProgramadoAsync(int pagoId, int usuarioId);
        Task<List<PagoProgramadoDTO>> ObtenerPagosProgramadosAsync(FiltroPagosProgramadosDTO filtro);
        Task<List<PagoProgramadoDTO>> ObtenerPagosProximosAsync(FiltroPagosProximosDTO filtro);
    }
}